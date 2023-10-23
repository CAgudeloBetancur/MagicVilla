using System.Net;

namespace MagicVilla_API.Models;

public class APIResponse
{
    public HttpStatusCode StatusCode { get; set; }

    public bool IsExitoso { get; set; } = true;
    public List<string> ErrorMessages { get; set; }

    public object Resultado { get; set; }
}
