# TextDeserializer
Project is trying to introduce a simple way how to map free form text on specific model class.

# How it works

## Data source
Parsing is getting text to parse through ``ITextSource`` interface.
```csharp
class TextSource : ITextSource
{
    public IEnumerable<string> GetPagesText()
    {
        var page1String = "random text containing property: 5";
        yield return page1String;
    }
}
```

## Parsing
Parsing is done through ``TextDocumentParser<>.Parse`` method. Generic is defininy model type into which text will be parsed.

```csharp
var parsedDocument = new TextDocumentParser<MyModel>().Parse(new TextSource());
Console.WriteLine(parsedDocument.Property);
```

## Model classes

### Model with simple types
In this example we'd like to get an integer from the sample above into our model class.

```csharp
[DeserializeByRegex("property: (?<Property>[0-9]+)")]
class MyModel
{
    public int Property { get; set; }
}
```

## Model with complex arrays
Library allows to parse tables in the text, lets consider input:

```text
Name | Value
property: 1
property: 2
property: 3
@Copyright xxx
```

```csharp
class CollectionMyModel
{
    [DeserializeCollectionByRegex("property", "Name | Value(.+)@Copyright")]
    public MyModel[] MyModel { get; set; }
}
```

In this example ``DeserializeCollectionByRegex`` attribute has two parameters.

1. parameter is defining regex how to detect a row in the table (when ``property`` string is found, than it's new row. 
2. parameter is defining how we should detect the table in a string. Everything within first group will be considered as text from table.

For successfull parsing is necessary to use also attribute DeserializeByRegex attribute annotating MyModel class, which will decide how row should be parsed.
