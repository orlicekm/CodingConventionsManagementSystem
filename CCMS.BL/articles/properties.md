# How to create additional properties

1. Create property class that implements ```IProperty```*[ref](/CodingConventionsManagementSystem/api/CCMS.BL.Services.EditorConfig.Properties.Base.IProperty.html)* interface. Implement interface members.
2. If property can be automatically imported from repository, make the class implement ```IImportable```*[ref](/CodingConventionsManagementSystem/api/CCMS.BL.Services.EditorConfig.Properties.Base.IImportable.html)* interface. Implement interface members.
3. If property can be checked on repository, make the class implement ```ICheckable```*[ref](/CodingConventionsManagementSystem/api/CCMS.BL.Services.EditorConfig.Properties.Base.ICheckable.html)* interface. Implement interface members.
4. All done! Property is created. There is no need to add property to *Inversion of Control*, it is done automatically using [reflection](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/reflection).

* *HINTS:*
  * Methods from ```PropertyExtensions```*[ref](/CodingConventionsManagementSystem/api/CCMS.BL.Services.EditorConfig.Properties.Helpers.PropertyExtensions.html)* class can be used to help implement some properties.
  * There is description for each interface member in [API documentation](/CodingConventionsManagementSystem/api/index.html), which can help with implementation.
  * Already implemented properties can be checked to help implement new ones.