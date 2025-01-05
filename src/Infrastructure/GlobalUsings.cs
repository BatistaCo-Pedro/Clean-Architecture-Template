// Global using directives

global using System.Collections.Concurrent;
global using System.Linq.Expressions;
global using System.Reflection;
global using System.Runtime.CompilerServices;
global using Application.Abstractions;
global using Application.Models.Entities;
global using Application.Models.Entities.TemplateTypeAggregate;
global using Infrastructure.Events;
global using Infrastructure.Implementations;
global using Infrastructure.Persistence;
global using Infrastructure.Persistence.Converters;
global using Infrastructure.Repositories;
global using MassTransit.Mediator;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
global using Microsoft.Extensions.DependencyInjection;
global using OnlyResult;
global using OnlyResult.Helpers;
global using OnlyValidationTypes;
global using Serilog;
global using Serilog.Events;