using DataManagement;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;

public class ConfigurableCollectionTests
{    
    static readonly IList<ConfigurableField> _testFields1 =  new ReadOnlyCollection<ConfigurableField>(
        new List<ConfigurableField> {new ConfigurableField("field11", "value11", typeof(string).FullName, new FieldMetadata()),
        new ConfigurableField("field12", "256", typeof(int).FullName, new FieldMetadata()),
        new ConfigurableField("fieldRepeated", "valueFrom1", typeof(string).FullName, new FieldMetadata())});
    static readonly IList<ConfigurableField> _testFields2 =  new ReadOnlyCollection<ConfigurableField>(
        new List<ConfigurableField> {new ConfigurableField("field21", "value21", typeof(string).FullName, new FieldMetadata()),
        new ConfigurableField("field22", "1024", typeof(int).FullName, new FieldMetadata()),
        new ConfigurableField("fieldRepeated", "valueFrom2", typeof(string).FullName, new FieldMetadata())});
    static readonly ConfigurableClassData _testConfigurable1 = new ConfigurableClassData("class1", _testFields1);
    static readonly ConfigurableClassData _testConfigurable2 = new ConfigurableClassData("class2", _testFields2);
    static readonly IList<ConfigurableClassData> _testDataForClasses = new ReadOnlyCollection<ConfigurableClassData>(
        new List<ConfigurableClassData>{_testConfigurable1, _testConfigurable2});
        
    
    [Test]
    public void TestDataValid()
    {
        Assert.That(_testDataForClasses.Where(element => element != null).Any());
        Assert.That(new List<string>{_testConfigurable1.ClassName, _testConfigurable2.ClassName}.Contains(_testDataForClasses.First().ClassName));
        Assert.That(_testDataForClasses.First(instance => instance.ClassName == "class1").Fields, Is.EqualTo(_testFields1));        
    }
    
    [Test]
    public void ConstructorTest()
    {
        var collection = new ConfigurablesCollection();
        Assert.That(collection.RegisteredConfigurables.Count, Is.EqualTo(0));
        collection = new ConfigurablesCollection(_testDataForClasses);
        Assert.That(collection.RegisteredConfigurables, Is.EqualTo(_testDataForClasses));
    }
    
    
    ConfigurablesCollection _collection;
    ConfigurablesCollection _collectionWithInitialData;
    
    [SetUp]
    public void TestSetup()
    {
        _collection = new ConfigurablesCollection(); 
        _collectionWithInitialData = new ConfigurablesCollection(_testDataForClasses);
    }
    
    
    [Test]
    public void SetFieldTest()
    {
        var className = "createdClass";
        var baseField = _testFields1.FirstOrDefault(entry => entry.Name == "fieldRepeated");
        
        _collection.SetRegisteredField(className, baseField);
        Assert.That(_collection.ClassRegistered(className));
        Assert.That(_collection.RegisteredConfigurables.Count, Is.EqualTo(1));
        Assert.That(_collection.RegisteredConfigurables.First().Fields.Count, Is.EqualTo(1));
        var entryFromCollection = _collection.RegisteredConfigurables.FirstOrDefault(entry => entry.ClassName == className);
        Assert.That(entryFromCollection.Fields.Contains(baseField));
        
        var updatedValue = "valueFromTest";
        var updatedField = new ConfigurableField(baseField.Name, updatedValue, baseField.Type, baseField.Metadata);
        _collection.SetRegisteredField(className, updatedField);
        Assert.That(_collection.RegisteredConfigurables.Count, Is.EqualTo(1));
        Assert.That(_collection.RegisteredConfigurables.First().Fields.Count, Is.EqualTo(1));
        var updatedEntryFromCollection = _collection.RegisteredConfigurables.FirstOrDefault(entry => entry.ClassName == className);
        Assert.That(updatedEntryFromCollection.Fields.Contains(updatedField));
    }
}
