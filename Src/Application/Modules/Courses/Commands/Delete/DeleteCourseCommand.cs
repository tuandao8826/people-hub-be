using Application.Common.Interfaces.Persistence;
using Application.Common.Requests;
using Application.Common.Responses;
using Application.Modules.Courses.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Courses.Commands.Delete;

public class DeleteCourseCommand : MultipleIdentiferRequest<Guid>, IRequest<MultipleIdentiferResponse>;