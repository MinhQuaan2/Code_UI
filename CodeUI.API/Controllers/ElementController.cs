using CodeUI.Data.Entity;
using CodeUI.Service.DTO.Request;
using CodeUI.Service.DTO.Request.ElementRequest;
using CodeUI.Service.DTO.Response;
using CodeUI.Service.DTO.Response.ElementResponses;
using CodeUI.Service.Exceptions;
using CodeUI.Service.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace CodeUI.API.Controllers
{
    [Route(Helpers.SettingVersionApi.ApiVersion)]
    [ApiController]
    public class ElementController : ControllerBase
    {
        private readonly IElementService _elementService;

        public ElementController(IElementService elementService)
        {
            _elementService = elementService;
        }


        ///<summary>
        /// Get all elements with filter
        /// </summary>
        [HttpGet("getAll")]
        public async Task<ActionResult<BaseResponsePagingViewModel<SimpleElementResponse>>> GetElements([FromQuery]SimpleElementResponse filter, [FromQuery]PagingRequest paging)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var accountId = FireBaseService.GetUserIdFromHeaderToken(accessToken);

            try
            {
                filter.Status = filter.Status?.ToUpper();
                var result = await _elementService.GetElements(filter, paging,accountId);
                return Ok(result);
            }
            catch(ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        ///<summary>
        /// Get all favorite elements with filter
        /// </summary>
        [HttpGet("getFavoriteElements")]
        public async Task<ActionResult<BaseResponsePagingViewModel<SimpleElementResponse>>> GetFavoriteElements([FromQuery]SimpleElementResponse filter, [FromQuery]PagingRequest paging)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var accountId = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (accountId == null)
                {
                    return Unauthorized();
                }

                filter.Status = filter.Status?.ToUpper();
                var result = await _elementService.GetFavoriteElements(filter,paging,accountId);
                return Ok(result);
            }
            catch(ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        ///<summary>
        /// Get random elements from free creators
        /// </summary>
        [HttpGet("getRandomElements")]
        public async Task<ActionResult<BaseResponsePagingViewModel<SimpleElementResponse>>> GetRandomFreeElements([FromQuery]SimpleElementResponse filter, [FromQuery]PagingRequest paging)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var accountId = FireBaseService.GetUserIdFromHeaderToken(accessToken);

            try
            {
                filter.Status = filter.Status?.ToUpper();
                var result = await _elementService.GetRandomFreeElements(filter, paging, accountId);
                return Ok(result);
            }
            catch(ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        ///<summary>
        /// Get Element by ID
        /// </summary>
        [HttpGet("getByID")]
        public async Task<ActionResult<BaseResponseViewModel<ElementResponse>>> GetElementByID(int id)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var accountId = FireBaseService.GetUserIdFromHeaderToken(accessToken);
            try
            {
                var result = await _elementService.GetElementByID(id, accountId);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        ///<summary>
        /// Create a new element project
        /// </summary>
        [Authorize(Roles ="SystemAdmin, FreeCreator, ProCreator, Moderator, ProPlusCreator")]
        [HttpPost("createElement")]
        public async Task<ActionResult<BaseResponseViewModel<ElementResponse>>> CreateElement([FromBody]CreateElementRequest request)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var accountId = FireBaseService.GetUserIdFromHeaderToken(accessToken);
            if (accountId == null)
            {
                return Unauthorized();
            }

            request.OwnerId = Guid.Parse(accountId);

            try
            {
                var result = await _elementService.CreateElement(request);
                return Ok(result);
            }
            catch(ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        ///<summary>
        /// Update an existing element
        ///</summary>
        ///<param name="eleID"> ID of the element to be updated</param>
        [Authorize(Roles = "SystemAdmin, FreeCreator, ProCreator, SystemAdmin, ProPlusCreator")]
        [HttpPut("updateElement")]
        public async Task<ActionResult<BaseResponseViewModel<SimpleElementResponse>>> UpdateElement(int eleID, UpdateElementRequest request)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var accountId = FireBaseService.GetUserIdFromHeaderToken(accessToken);
            if (accountId == null)
            {
                return Unauthorized();
            }
            try
            {
                var result = await _elementService.UpdateElement(eleID,accountId,request);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        /////<summary>
        ///// Update multiple elements' statuses at the same time
        ///// </summary>
        //[Authorize(Roles = "SystemAdmin, Moderator")]
        //[HttpPut("updateElementStatus")]
        //public async Task<ActionResult<List<ElementResponse>>> UpdateElementStatus(UpdateStatusRequest request)
        //{
        //    try
        //    {
        //        var result = await _elementService.UpdateElementStatuses(request);
        //        return Ok(result);
        //    }
        //    catch (ErrorResponse ex)
        //    {
        //        return BadRequest(ex.Error);
        //    }
        //}

        ///<summary>
        /// Post an element to pending list, waiting for moderators' approval
        /// </summary>
        /// <param name="id"> Element's ID</param>
        [Authorize(Roles = "FreeCreator, ProCreator, SystemAdmin, ProPlusCreator")]
        [HttpPost("postElement")]
        public async Task<ActionResult<BaseResponseViewModel<ElementResponse>>> PostElement([FromQuery]int id)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var accountId = FireBaseService.GetUserIdFromHeaderToken(accessToken);
            if (accountId == null)
            {
                return Unauthorized();
            }

            try
            {
                var result = await _elementService.PostElement(id,accountId);
                return Ok(result);
                
            }
            catch(ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        
        ///<summary>
        /// Delete/disable an element of your own
        /// </summary>
        /// <param name="id"> Element's ID</param>
        [Authorize(Roles = "FreeCreator, ProCreator, ProPlusCreator")]
        [HttpDelete("deleteElement")]
        public async Task<ActionResult<ElementResponse>> DeleteElement([FromQuery]int id)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var accountId = FireBaseService.GetUserIdFromHeaderToken(accessToken);
            if (accountId == null)
            {
                return Unauthorized();
            }

            try
            {
                var result = await _elementService.DeleteElement(id,accountId);
                return Ok(result);
                
            }
            catch(ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
    }
}
