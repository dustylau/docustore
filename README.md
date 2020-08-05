# docustore
## A database file system API

Docustore is a an API that manages file/documents using a database for indexing and storage.

Instead of depending on the host file system to store the file/folder system I chose to write a database file system because it was more fun.

The solution is created using ASP.net Core MVC and Entity Framework and is split out into several projects to allow for better extensiblility and testing.

Core classes and interfaces are stored in ".Core*" projects that are used in this implementation.
These core compononents can be implemented by additional projects and bootstrapped using the IOC bootstrapper component.

This solution is using Entity Framework and SQLite as the document storage repository but a repository can be created from any database or storage mechanism as long as it can be wrapped up in a class implementing the IFileSystem interface.

The main file system component can be reconfigured in the bootstrapper (as it is now) or the bootstrapper could be updated to scan for file system components and load them at run time using the Lamar container scanning function.


### Future Improvements
 - OData query capability: currently using dynamic linq library to allow for item searching but I prefer OData
 - Tag based search: the system already allows for item tagging but tag based searching is desirable
 - File content indexing and search: full text index and search via document query implementation (Elastic Search)
 - File/Folder security based authorization
