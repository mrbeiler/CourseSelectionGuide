using System.Text.Json;
using Microsoft.JSInterop;

public class SessionStorageService
{
    private readonly IJSRuntime _js;
    private const string Key = "user_session";

    public SessionStorageService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task SaveAsync(UserSession session)
    {
        var dto = session.ToDto();
        var json = JsonSerializer.Serialize(dto);

        await _js.InvokeVoidAsync("sessionStorage.setItem", Key, json);
    }

    public async Task<UserSessionDto?> LoadAsync()
    {
        var json = await _js.InvokeAsync<string>("sessionStorage.getItem", Key);

        if (string.IsNullOrEmpty(json))
            return null;

        return JsonSerializer.Deserialize<UserSessionDto>(json);
    }

    public async Task ClearAsync()
    {
        await _js.InvokeVoidAsync("sessionStorage.removeItem", Key);
    }
}