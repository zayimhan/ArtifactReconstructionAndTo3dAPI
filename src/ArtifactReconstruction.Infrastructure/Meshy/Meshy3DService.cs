using System.Net.Http.Json;
using System.Text.Json;
using ArtifactReconstruction.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace ArtifactReconstruction.Infrastructure.Meshy;

public class Meshy3DService : IMeshy3DService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;

    public Meshy3DService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _config = config;

        var apiKey = config["Meshy:ApiKey"]
                     ?? throw new Exception("Meshy API key missing");

        _http.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
    }

    public async Task<string> Generate3DAsync(string imageUrl, string prompt)
    {
        Console.WriteLine($"[1/3] Meshy'ye yüksek kaliteli istek gönderiliyor... Görsel: {imageUrl}");

        // Yüksek kaliteli doku ve model yapısı için payload güncellendi
        var payload = new
        {
            image_url = imageUrl,
            enable_pbr = true,           // DOKU İÇİN ŞART: Realistic doku setini (Albedo, Roughness, Normal) oluşturur.
            mode = "high",               // KALİTE: 'preview' yerine 'high' kullanılarak tam çözünürlük sağlanır.
            art_style = "realistic",     // TARZ: Arkeolojik objeler için gerçekçi doku projeksiyonu sağlar.
            should_remesh = true,        // YAPI: Poligon yapısını doku yerleşimine (UV) daha uygun hale getirir.
            negative_prompt = "low poly, blur, distorted, cartoon, toy" // İstenmeyen tarzları engeller.
        };

        var response = await _http.PostAsJsonAsync(
            "https://api.meshy.ai/v1/image-to-3d",
            payload
        );

        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception("Meshy Start Error → " + json);

        using var doc = JsonDocument.Parse(json);
        string taskId = "";

        if (doc.RootElement.TryGetProperty("result", out var resultElement))
        {
            taskId = resultElement.ValueKind == JsonValueKind.String 
                ? resultElement.GetString()! 
                : resultElement.GetProperty("task_id").GetString()!;
        }
        else if (doc.RootElement.TryGetProperty("task_id", out var taskIdElement))
        {
            taskId = taskIdElement.GetString()!;
        }

        if (string.IsNullOrEmpty(taskId))
            throw new Exception($"Task ID alınamadı. Yanıt: {json}");

        Console.WriteLine($"[2/3] Meshy işlemi kabul etti! Görev ID: {taskId}");
        return taskId;
    }

    public async Task<string> WaitForCompletionAsync(string taskId)
    {
        Console.WriteLine($"[3/3] Yüksek kaliteli model ve dokular oluşturuluyor (Task: {taskId})...");
        Console.WriteLine("Not: 'high' modu doku detayı nedeniyle 10-15 dakika sürebilir.");

        // High mode için deneme sayısını 240'a (20 dakika) çıkarıyoruz
        for (int i = 0; i < 240; i++)
        {
            await Task.Delay(5000); 

            var response = await _http.GetAsync(
                $"https://api.meshy.ai/v1/image-to-3d/{taskId}"
            );

            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception("Meshy Status Error → " + json);

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var status = root.GetProperty("status").GetString()?.ToUpper();
            
            // İlerleme durumunu yüzdelik olarak görmeye çalışalım (Eğer API dönüyorsa)
            string progress = root.TryGetProperty("progress", out var p) ? $" (%{p})" : "";
            Console.WriteLine($"--- Deneme {i + 1}: Durum = {status}{progress}");

            if (status == "SUCCEEDED" || status == "COMPLETED")
            {
                Console.WriteLine("!!! YÜKSEK KALİTELİ MODEL TAMAMLANDI !!!");
                
                if (root.TryGetProperty("model_url", out var modelUrlProp))
                    return modelUrlProp.GetString()!;
                
                return root.GetProperty("result").GetProperty("model_url").GetString()!;
            }

            if (status == "FAILED")
            {
                throw new Exception("Meshy failed → " + json);
            }
        }

        throw new Exception("Meshy timeout - Yüksek kaliteli işlem 20 dakikadan uzun sürdü.");
    }
}