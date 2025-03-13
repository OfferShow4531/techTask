Инструкция по запуску проекта RESTfulAPI(Technical Task)

1. В проекте собраны следующие библиотеки:
    Dapper
    SignalR
    Hangfire
    JwtBearerHandler
    OpenApi
    MediatR
    FluentValidation
    AutoMapper
    Serilog
    PostgreSql

2. Привязка идёт по ядру СУБД (localhost) c заданными настройками в файле appsettings.json
3. Для управления проектом необходимо скачать библиотеки выше и настроить базу данных (backup и коллекция прилигаются в репозитории)
4. Проект поднят на порту 7270, но его можно изменить на любой доступный в параметрах launchSettings.json
5. Помимо работы с POSTMAN проект поддерживает прямое обращение через Swagger UI (обращение по ссылке: https://localhost:7270/swagger/index.html)
6. Проект был поднят на версии .Net 8.0 и использует последнюю версию PostgreSQL
7. Не были использованы библиотеки: RabbitMQ ввиду отсутствия платформы Docker.