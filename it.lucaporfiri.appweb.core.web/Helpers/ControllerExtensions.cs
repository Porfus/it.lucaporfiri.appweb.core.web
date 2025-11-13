using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.IO;
using System.Threading.Tasks;

namespace it.lucaporfiri.appweb.core.web.Helpers
{
    public static class ControllerExtensions
    {
        /// <summary>
        /// Renderizza una vista parziale (Partial View) in una stringa HTML.
        /// Questo metodo è asincrono.
        /// </summary>
        /// <param name="controller">Il controller da cui viene chiamato il metodo.</param>
        /// <param name="viewName">Il nome o il percorso della vista parziale (es. "_MyPartial").</param>
        /// <param name="model">L'oggetto modello da passare alla vista parziale.</param>
        /// <returns>Una stringa contenente l'HTML renderizzato.</returns>
        public static async Task<string> RenderViewToStringAsync(this Controller controller, string viewName, object model)
        {
            // Se il nome della vista non è specificato, usa il nome dell'azione corrente
            if (string.IsNullOrEmpty(viewName))
            {
                viewName = controller.ControllerContext.ActionDescriptor.ActionName;
            }

            // Assegna il modello al ViewData del controller
            controller.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                // Trova la vista parziale usando il motore di viste composito
                // IViewEngine viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
                // La riga sopra è il modo "corretto" ma più verboso. Spesso si può accedere direttamente.
                var viewEngine = controller.HttpContext.RequestServices.GetRequiredService<ICompositeViewEngine>();

                ViewEngineResult viewResult = viewEngine.FindView(controller.ControllerContext, viewName, isMainPage: false);

                // Se la vista non viene trovata, lancia un'eccezione chiara
                if (viewResult.View == null)
                {
                    throw new ArgumentNullException($"{viewName} does not match any available view");
                }

                // Crea il contesto della vista
                var viewContext = new ViewContext(
                    controller.ControllerContext,
                    viewResult.View,
                    controller.ViewData,
                    controller.TempData,
                    sw,
                    new HtmlHelperOptions()
                );

                // Renderizza la vista in modo asincrono
                await viewResult.View.RenderAsync(viewContext);

                // Restituisce la stringa dall'oggetto StringWriter
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}