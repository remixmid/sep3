using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using DTOs.ChatDTOs;
using DTOs.UserActionRequests;
using Microsoft.AspNetCore.Mvc;

namespace API.CoreConnection;

public class MessageClient {
    private readonly HttpClient httpClient;

    public MessageClient(HttpClient client) {
        httpClient = client;
    }

    /*
     * Corresponds to POST /api/messages
     */
    public async ValueTask<MessageDTO> SendMessage(SendMessageRequest req) {
        var res = await httpClient.PostAsJsonAsync("/", req);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<MessageDTO>()
            ?? throw new HttpRequestException();
    }

    /*
     * Corresponds to PATCH /api/messages/{messageId}
     */
    public async ValueTask<MessageDTO> EditMessage(String id, EditMessageRequest req) {
        var res = await httpClient.PatchAsJsonAsync($"/{id}", req);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<MessageDTO>()
            ?? throw new HttpRequestException();
    }

    /*
     * Corresponds to DELETE /api/messages/{messageId}?userId={id}&forAll={bool}
     */
    public async Task DeleteMessage(String id, DeleteMessageRequest req) {
        // TODO: I think this is correct? Although I am honestly not sure.
        var res = await httpClient.DeleteAsync($"/{id}?userId={req.UserId}&forAll={req.ForAll}");
        res.EnsureSuccessStatusCode();
    }

    /*
     * Corresponds to DELETE /api/messages
     */
    public async Task DeleteManyMessages(DeleteMessageRequest req) {
        // TODO: I cannot add a body to DeleteFromJsonAsync, so this was the best alternative I could find.
        using var request = new HttpRequestMessage(HttpMethod.Delete, "/") {
        Content = JsonContent.Create(req)
        };
        var res = await httpClient.SendAsync(request);
        res.EnsureSuccessStatusCode();
    }
}
