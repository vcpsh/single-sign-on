using System.Threading.Tasks;

namespace sh.vcp.sso.server.Utilities
{
    public interface IViewRenderService
    {
        Task<string> RenderToString(string viewName, object model);
    }
}