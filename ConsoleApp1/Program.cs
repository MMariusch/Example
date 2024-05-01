using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Example
{
	internal class Program
	{
		static void Main(string[] args)
		{
			var test = new TestClass();
			test.SaveToFile();
			test.LoadFromFile();
		}
	}

	public class TestClass
	{
		List<BaseClass> objects = new List<BaseClass>();

		public TestClass() { }

		public void SaveToFile()
		{
			objects.Add(new BClass("someString"));
			objects.Add(new AClass("anotherString"));
			var xmlSerializer = new XmlSerializer(typeof(List<BaseClass>));
			using (var writer = new StreamWriter("SaveList.xml"))
			{
				xmlSerializer.Serialize(writer, objects);
			}
		}

		public void LoadFromFile()
		{
			if (File.Exists("SaveList.xml"))
			{
				using (var reader = new StreamReader("SaveList.xml"))
				{
					var deserializedList = new XmlSerializer(typeof(List<BaseClass>)).Deserialize(reader) as List<BaseClass>;
					if (deserializedList != null && deserializedList.Count > 0)
					{
						objects = deserializedList;
					}
				}
			}
		}
	}

	[XmlInclude(typeof(AClass))]
	[XmlInclude(typeof(BClass))]
	[XmlSchemaProvider(nameof(GetSchema))]
	public abstract class BaseClass : IXmlSerializable
	{
		internal static XmlSchema Schema { get; } = new XmlSchema()
		{
			Items = {
			  new XmlSchemaComplexType() { Name = "BaseClass" },
			  new XmlSchemaComplexType() {
				Name = "AClass", ContentModel = new XmlSchemaComplexContent() {
				  Content = new XmlSchemaComplexContentExtension() { BaseTypeName = new XmlQualifiedName("BaseClass"), Attributes = {}, }, }, },
			  new XmlSchemaComplexType() {
				Name = "BClass", ContentModel = new XmlSchemaComplexContent() {
				  Content = new XmlSchemaComplexContentExtension() { BaseTypeName = new XmlQualifiedName("BaseClass"), Attributes = {}, }, }, },
			}
		};

		public BaseClass() { }

		public XmlSchema GetSchema()
		{
			return null;
		}

		public static XmlQualifiedName GetSchema(XmlSchemaSet xs)
		{
			xs.Add(BaseClass.Schema);
			return new XmlQualifiedName("BaseClass");
		}

		public virtual void ReadXml(XmlReader reader)
		{
			Console.Write("It shouldn't be triggered.");
		}
		public virtual void WriteXml(XmlWriter writer) { }

	}

	[XmlSchemaProvider(nameof(GetSchema))]
	public class AClass : BaseClass
	{
		private string _stringVar;
		public string StringVar { get => _stringVar; private set => _stringVar = value; }
		public AClass() { }
		public AClass(string stringVar)
		{
			_stringVar = stringVar;
		}
		public static new XmlQualifiedName GetSchema(XmlSchemaSet xs)
		{
			xs.Add(BaseClass.Schema);
			return new XmlQualifiedName("AClass");
		}

		public override void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();
			var anyElements = !reader.IsEmptyElement;
			reader.ReadStartElement();
			if (anyElements)
			{
				_stringVar = reader.ReadElementContentAsString("AStringVar", "");
				reader.ReadEndElement();
			}
		}

		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteAttributeString("xsi", "type", null, "AClass");
			writer.WriteElementString("AStringVar", _stringVar);
		}
	}

	[XmlSchemaProvider(nameof(GetSchema))]
	public class BClass : BaseClass
	{
		private string _stringVar;
		public string StringVar { get => _stringVar; private set => _stringVar = value; }
		public BClass() { }
		public BClass(string stringVar)
		{
			_stringVar = stringVar;
		}
		public static new XmlQualifiedName GetSchema(XmlSchemaSet xs)
		{
			xs.Add(BaseClass.Schema);
			return new XmlQualifiedName("BClass");
		}

		public override void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();
			var anyElements = !reader.IsEmptyElement;
			reader.ReadStartElement();
			if (anyElements)
			{
				_stringVar = reader.ReadElementContentAsString("BStringVar", "");
				reader.ReadEndElement();
			}
		}

		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteAttributeString("xsi", "type", null, "BClass");
			writer.WriteElementString("BStringVar", _stringVar);
		}
	}
}
