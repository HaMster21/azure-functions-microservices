# Services with Azure Functions

This is a demo project of two services cooperating with each other to provide products and catalogs. Users want to have a catalog per product category, so whenever a product is created an the catalog for its category doesn't exist, it is created by the catalog service. Interactions are modeled with Azure Service Bus. Each services stores data is an MSSQL server.

# Deployment

Just publish each function project to an Azure Subscription. ServiceBus namespace, SQLServer and AppInsights workspace must be created manually. The latter is useful for tracing and debugging, since every services interaction is grouped by CorrelationID.
