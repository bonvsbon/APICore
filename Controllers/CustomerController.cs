using System;
using System.Data;
using System.Net.Cache;
using Microsoft.AspNetCore.Mvc;
using static APICore.Models.appSetting;
using APICore.Common;
using APICore.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;

namespace APICore.Controllers {
    [ApiController]
    [Route ("api/[controller]/[action]")]
    [Produces("application/json")]
    public class CustomerController : ControllerBase {
        StateConfigs state = new StateConfigs();
        Functional func;
        RequestDataModel reqModel;
        CustomerModel customer;
        public CustomerController(IOptions<StateConfigs> config) {
            func = new Functional();
            customer = new CustomerModel(config);
            reqModel = new RequestDataModel();
        }

        // [Authorize]
        [HttpPost]
        public IActionResult index([FromBody] RequestModel request) {
            ResponseModel result = new ResponseModel();
            try
            {
                result._data = customer.REST_CustomerInformationbyNationID(request.data.Id);
                result._errorMessage = "";
                if(result._data.Rows.Count == 0)
                {
                    result._statusCode = result.GetStateHttp(ResponseModel.StatusHttp.NotFound);
                    return NotFound(result);
                }
                result._statusCode = result.GetStateHttp(ResponseModel.StatusHttp.OK);
                return Ok(result);
            } 
            catch (Exception e)
            {
                result._statusCode = result.GetStateHttp(ResponseModel.StatusHttp.InternalError);
                result._errorMessage = e.Message;
                return BadRequest(result);
            }

        }

        // [Authorize]
        [HttpPost]
        public IActionResult purchaseHistory([FromBody] RequestModel request) {
            ResponseModel result = new ResponseModel();
            try
            {
                result._data = customer.REST_PurchaseHistory(request.data.Id);
                result._errorMessage = "";
                if(result._data.Rows.Count == 0)
                {
                    result._statusCode = result.GetStateHttp(ResponseModel.StatusHttp.NotFound);
                    return NotFound(result);
                }
                result._statusCode = result.GetStateHttp(ResponseModel.StatusHttp.OK);
                return Ok(result);
            } 
            catch (Exception e)
            {
                result._statusCode = result.GetStateHttp(ResponseModel.StatusHttp.InternalError);
                result._errorMessage = e.Message;
                return BadRequest(result);
            }
        }

        // [Authorize]
        [HttpPost]
        public IActionResult installmentTable([FromBody] RequestModel request) {
            ResponseModel result = new ResponseModel();
            try
            {
                result._data = customer.REST_InstallmentTable(request.data.AgreementNo);
                result._errorMessage = "";
                if(result._data.Rows.Count == 0)
                {
                    result._statusCode = result.GetStateHttp(ResponseModel.StatusHttp.NotFound);
                    return NotFound(result);
                }
                result._statusCode = result.GetStateHttp(ResponseModel.StatusHttp.OK);
                return Ok(result);
            } 
            catch (Exception e)
            {
                result._statusCode = result.GetStateHttp(ResponseModel.StatusHttp.InternalError);
                result._errorMessage = e.Message;
                return BadRequest(result);
            }
        }

    }
}