using Contacts.Abstractions.Commands;
using Contacts.Abstractions.Queries;
using Contacts.Application.Exceptions;
using Contacts.Contracts.Commands;
using Contacts.Contracts.Queries;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Contacts.Api;

public static class Endpoints
{
    public static RouteGroupBuilder MapContactsEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("", async Task<Ok<ContactsResponse>> (
                [AsParameters] GetContactsParams queryParams,
                [FromServices] IGetContactsQuery contactsQuery,
                [FromServices] IGetContactsJobsQuery jobsQuery,
                CancellationToken cancellationToken) =>
            {
                var pagedContacts = await contactsQuery.GetContactsAsync(
                    new GetContactsParams(queryParams.PageNumber, queryParams.PageSize, queryParams.NameSearch,
                        queryParams.JobTitleSearch),
                    cancellationToken);

                var jobs = await jobsQuery.GetContactsJobsAsync(queryParams.NameSearch, cancellationToken);

                return TypedResults.Ok(new ContactsResponse(pagedContacts, jobs));
            })
            .WithName("GetContacts")
            .Produces<ContactsResponse>();

        group.MapGet("/{id:guid}", async Task<Results<Ok<GetContactDto>, NotFound>> (
                Guid id,
                [FromServices] IGetContactByIdQuery query,
                CancellationToken cancellationToken) =>
            {
                var result = await query.GetContactAsync(id, cancellationToken);
                return result is not null
                    ? TypedResults.Ok(result)
                    : TypedResults.NotFound();
            })
            .WithName("GetContactById")
            .Produces<GetContactDto>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("",
                async Task<Results<CreatedAtRoute, BadRequest<ValidationProblemDetails>>> (
                    [FromBody] SaveContactDto dto,
                    [FromServices] ISaveContactCommand command,
                    CancellationToken cancellationToken) =>
                {
                    var newContactId = await command.ExecuteAsync(dto, cancellationToken);
                    return TypedResults.CreatedAtRoute("GetContactById", new { id = newContactId });
                })
            .WithName("CreateContact")
            .Produces(StatusCodes.Status201Created)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapPut("/{id:guid}", async Task<Results<NoContent, NotFound, BadRequest<ValidationProblemDetails>>> (
                Guid id,
                [FromBody] UpdateContactDto dto,
                [FromServices] IUpdateContactCommand command,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    await command.ExecuteAsync(id, dto, cancellationToken);
                    return TypedResults.NoContent();
                }
                catch (NotFoundException)
                {
                    return TypedResults.NotFound();
                }
            })
            .WithName("UpdateContact")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapDelete("/{id:guid}", async Task<Results<NoContent, NotFound>> (
                Guid id,
                [FromServices] IDeleteContactCommand command,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    await command.ExecuteAsync(new DeleteContactDto(id), cancellationToken);
                    return TypedResults.NoContent();
                }
                catch (NotFoundException)
                {
                    return TypedResults.NotFound();
                }
            })
            .WithName("DeleteContact")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        group.WithOpenApi();
        return group;
    }
}