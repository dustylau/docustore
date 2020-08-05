# docustore
## A database file system API

Docustore is a an API that manages file/documents using a database for indexing and storage.

The solution is created using ASP.net Core MVC and Entity Framework.
The solution is split out into several projects to allow for better extensiblility and testing.

Core classes and interfaces are stored in ".Core*" projects that are used in this implementation
These core compononents can be implemented by additional projects and bootstrapped using the IOC bootstrapper component.

This solution is using Entity Framework and SQLite as the document storage repository but a repository can be created from any database or storage mechanism as long as it can be wrapped up in a class implementing the IFileSystem interface.
