using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xero.NetStandard.OAuth2.Api;
using Xero.NetStandard.OAuth2.Model.Files;
using BlazorFilesApp.Shared;
using System.IO;
using Xero.NetStandard.OAuth2.Client;
using Xero.NetStandard.OAuth2.Model.Accounting;
using Microsoft.Extensions.Logging;
using AccountingApiWrapper = XeroServices.XeroServices;

namespace BlazorFilesApp.Server.Controllers
{
    public class XeroController : Controller
    {
        private readonly AccountingApiWrapper _accountingApi;

        public XeroController(AccountingApiWrapper accountingApi)
        {
            _accountingApi = accountingApi;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("files-get")]
        public async Task<IActionResult> GetFiles()
        {
            FilesApi filesApi = new();
            string accessToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", string.Empty);
            string tenantId = await GetTenantId(accessToken);
            Files response = await filesApi.GetFilesAsync(accessToken, tenantId);
            List<FileObject> filesItems = response.Items;
            return Ok(filesItems);
        }

        [HttpGet]
        [Route("getfolders")]
        public async Task<IActionResult> GetFolders()
        {
            FilesApi FilesApi = new();
            string accessToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", string.Empty);
            string tenantId = await GetTenantId(accessToken);
            List<Folder> response = await FilesApi.GetFoldersAsync(accessToken, tenantId);
            return Ok(response);
        }

        [HttpGet]
        [Route("invoices-get")]
        public async Task<IActionResult> GetInvoices()
        {
            AccountingApi accountingApi = new();
            string accessToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", string.Empty);
            string tenantId = await GetTenantId(accessToken);
            Invoices response = await accountingApi.GetInvoicesAsync(accessToken, tenantId);
            return Ok(response);
        }

        [HttpGet]
        [Route("file-get")]
        public async Task<IActionResult> GetFile(Guid id)
        {
            FilesApi FilesApi = new();
            string accessToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", string.Empty);
            string tenantId = await GetTenantId(accessToken);
            Stream response = await FilesApi.GetFileContentAsync(accessToken, tenantId, id);
            MemoryStream memoryStream = new();
            response.CopyTo(memoryStream);
            return Ok(memoryStream.ToArray());
        }

        [HttpGet]
        [Route("get-association")]
        public async Task<IActionResult> GetAssociation(Guid id)
        {
            FilesApi FilesApi = new();
            string accessToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", string.Empty);
            string tenantId = await GetTenantId(accessToken);
            List<Association> response = await FilesApi.GetFileAssociationsAsync(accessToken, tenantId, id);
            return Ok(response);
        }

        [HttpPost]
        [Route("file-upload")]
        public async Task<IActionResult> UploadFile([FromBody] XeroUpload file)
        {
            FilesApi FilesApi = new();
            string accessToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", string.Empty);
            string tenantId = await GetTenantId(accessToken);
            if (file.FolderId == null || file.FolderId == Guid.Empty)
            {
                await FilesApi.UploadFileAsync(accessToken, tenantId, file.FileContent, file.FileName, file.FileName, file.ContentType);
            }
            else
            {
                await FilesApi.UploadFileToFolderAsync(accessToken, tenantId, file.FolderId ?? Guid.Empty, file.FileContent, file.FileName, file.FileName, file.ContentType);
            }
            return Redirect("/");
        }


        [HttpPost]
        [Route("folder-create")]
        public async Task<IActionResult> CreateFolder([FromBody] XeroFolder createFolder)
        {
            FilesApi FilesApi = new();
            string accessToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", string.Empty);
            string tenantId = await GetTenantId(accessToken);
            Folder folder = new();
            folder.Name = createFolder.Name;
            await FilesApi.CreateFolderAsync(accessToken, tenantId, folder);
            return Redirect("/");
        }

        [HttpPost]
        [Route("deletefolder")]
        public async Task<IActionResult> DeleteFolder([FromBody] Folder folder)
        {
            FilesApi FilesApi = new();
            string accessToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", string.Empty);
            string tenantId = await GetTenantId(accessToken);
            await FilesApi.DeleteFolderAsync(accessToken, tenantId, folder.Id ?? Guid.Empty);
            return Redirect("/");
        }
        
        [HttpPost]
        [Route("invoice-delete")]
        public async Task<IActionResult> InvoiceDelete([FromBody] Invoice invoice)
        {
            string accessToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", string.Empty);
            string tenantId = await GetTenantId(accessToken);
            //AccountingApi accountingApi = new();
            //accountingApi.Invoice
            if (invoice.InvoiceID is not null)
            {
                var invoices = new Invoices()
                {
                    _Invoices = new List<Invoice> { invoice }
                };
                _accountingApi.SetAuth(accessToken, tenantId);
                _accountingApi.VoidInvoices(invoices);
            }
                

            //await accountingApi.UpdateInvoiceAsync(accessToken, tenantId, invoice.InvoiceID ?? Guid.Empty);
            return Redirect("/");
        }

        [HttpPost]
        [Route("deleteassociation")]
        public async Task<IActionResult> DeleteAssociation([FromBody] Association association)
        {
            FilesApi FilesApi = new();
            string accessToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", string.Empty);
            string tenantId = await GetTenantId(accessToken);
            await FilesApi.DeleteFileAssociationAsync(accessToken, tenantId, association.FileId ?? Guid.Empty, association.ObjectId ?? Guid.Empty);
            return Redirect("/");
        }

        [HttpPost]
        [Route("deletefile")]
        public async Task<IActionResult> DeleteFile([FromBody] FileObject file)
        {
            FilesApi FilesApi = new();
            string accessToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", string.Empty);
            string tenantId = await GetTenantId(accessToken);
            await FilesApi.DeleteFileAsync(accessToken, tenantId, file.Id ?? Guid.Empty);
            return Redirect("/");
        }

        [HttpPost]
        [Route("createassociation")]
        public async Task<IActionResult> CreateAssociation([FromBody] Association association)
        {
            FilesApi FilesApi = new();
            string accessToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", string.Empty);
            string tenantId = await GetTenantId(accessToken);
            await FilesApi.CreateFileAssociationAsync(accessToken, tenantId, association.FileId ?? Guid.Empty, association);
            return Redirect("/");
        }


        [HttpGet]
        [Route("getassociations")]
        public async Task<IActionResult> GetAssociations(Guid id)
        {
            FilesApi filesApi = new();
            string accessToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", string.Empty);
            string tenantId = await GetTenantId(accessToken);
            List<Association> response = await filesApi.GetFileAssociationsAsync(accessToken, tenantId, id);
            return Ok(response);
        }

        [HttpGet]
        [Route("getconnection")]
        public async Task<IActionResult> GetConnection()
        {
            string accessToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", string.Empty);
            await GetTenantId(accessToken);

            HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            List<Connection> connections = await client.GetFromJsonAsync<List<Connection>>("https://api.xero.com/connections");
            return Ok(connections.FirstOrDefault());
        }

        public async Task<string> GetTenantId(string accessToken)
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            List<Connection> connections = await client.GetFromJsonAsync<List<Connection>>("https://api.xero.com/connections");
            string tenantId = connections.FirstOrDefault().TenantId;
            return tenantId;
        }
    }

}
