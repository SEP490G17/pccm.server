﻿using Application.DTOs;
using Application.Handler.Services;
using Application.SpecParams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ServiceController : BaseApiController
    {
        public ServiceController() { }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetService([FromQuery] BaseSpecParam baseSpecParam, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new List.Query() { BaseSpecParam = baseSpecParam }, ct));
        }

        [AllowAnonymous]
        [HttpGet("{id}")]

        public async Task<IActionResult> GetService(int id, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Detail.Query() { Id = id }, ct));
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> PostService([FromBody] ServiceDto service, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Create.Command() { Service = service }, ct));
        }

        [AllowAnonymous]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateService(int id, ServiceDto updatedService)
        {
            updatedService.Id = id;
            return HandleResult(await Mediator.Send(new Edit.Command() { Service = updatedService }));
        }

        [AllowAnonymous]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command() { Id = id }));
        }
    }
}
