# Warehouse Management System

Приложение для управления складом, разработанное с использованием Blazor и ASP.NET Core Web API.

## Функциональность
- Просмотр и фильтрация складских запасов  
- Управление ресурсами и единицами измерения  
- Создание, редактирование и архивирование клиентов  
- Управление отгрузками (создание, подписание, отмена, удаление)  
- Уведомления и диалоги через MudBlazor  

## Архитектура
- **Frontend**: Blazor + MudBlazor  
- **Backend**: ASP.NET Core Web API  
- **База данных**: Microsoft SQL Server  
- **ORM**: Entity Framework Core  

## Технологии
- .NET 8  
- Blazor  
- MudBlazor  
- ASP.NET Core  
- Entity Framework Core  
- SQL Server  
- FluentValidation  
- AutoMapper  

## Структура проекта
- `WarehouseManagement.Api` — backend (контроллеры, сервисы, EF Core, миграции)  
- `WarehouseManagement.Blazor` — frontend (страницы Blazor, компоненты, UI на MudBlazor)  
- `WarehouseManagement.Application` — слой прикладной логики (сервисы, DTO, валидация, маппинги, интерфейсы)  
- `WarehouseManagement.Domain` — доменные сущности и бизнес-правила  
- `WarehouseManagement.Infrastructure` — доступ к данным, репозитории и интеграции с внешними сервисами  
