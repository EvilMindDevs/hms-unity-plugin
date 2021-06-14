using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace HmsPlugin
{
    public class HMSCloudDBSettingsDrawer : VerticalSequenceDrawer
    {
        private TextField.TextField packageNameTextField;
        private Label.Label jsonPathLabel;

        private Root rootJson;

        public HMSCloudDBSettingsDrawer()
        {
            packageNameTextField = new TextField.TextField("Java Package Name: ", "", OnPackageNameChanged); //TODO: reference wiki page to how to obtain java package name.
            jsonPathLabel = new Label.Label();
            SetupSequence();
        }

        private void OnPackageNameChanged(string value)
        {

        }

        private void OnJsonFileSelected()
        {
            string path = EditorUtility.OpenFilePanel("Choose a json File", "", "json");
            if (!string.IsNullOrEmpty(path))
            {
                rootJson = JsonUtility.FromJson<Root>(File.ReadAllText(path));
                jsonPathLabel.SetText(Path.GetFileName(path));
            }
        }

        private void SetupSequence()
        {
            AddDrawer(new HorizontalSequenceDrawer(new HorizontalLine(), new Label.Label("Creating C# Model").SetBold(true), new HorizontalLine()));
            AddDrawer(new Space(3));
            AddDrawer(packageNameTextField);
            AddDrawer(new Space(3));
            AddDrawer(new HorizontalSequenceDrawer(new Label.Label("Select Json File"), new Space(3), new Button.Button("Select", OnJsonFileSelected).SetWidth(100), jsonPathLabel));
            AddDrawer(new HorizontalSequenceDrawer(new Spacer(), new Button.Button("Generate C# Models", OnGenerateButton).SetWidth(250), new Spacer()));
            AddDrawer(new HorizontalLine());
            AddDrawer(new Space(5));
        }

        private void OnGenerateButton()
        {
            if (rootJson != null && !string.IsNullOrEmpty(packageNameTextField.GetCurrentText()))
            {
                foreach (var item in rootJson.objectTypes)
                {
                    using (var file = File.CreateText(Application.dataPath + "/Huawei/Scripts/CloudDB/" + item.objectTypeName + ".cs"))
                    {
                        file.WriteLine("using HuaweiMobileServices.CloudDB;");
                        file.WriteLine("using HuaweiMobileServices.Utils;");
                        file.WriteLine("using UnityEngine;\n");
                        file.WriteLine("using System;\n");
                        file.WriteLine("namespace HmsPlugin\n{");
                        file.WriteLine("\tpublic class " + item.objectTypeName + " : JavaObjectWrapper, ICloudDBZoneObject\n\t{");
                        file.WriteLine("\t\tpublic " + item.objectTypeName + "() : base(\"" + packageNameTextField.GetCurrentText() + "\") { }");
                        file.WriteLine("\t\tpublic " + item.objectTypeName + "(AndroidJavaObject javaObject) : base(javaObject) { }");
                        foreach (var field in item.fields)
                        {
                            file.WriteLine("\t\tprivate " + GetFieldType(field.fieldType) + " " + field.fieldName + ";");
                        }
                        file.WriteLine("");

                        foreach (var field in item.fields)
                        {
                            file.WriteLine(WriteProperty(field));
                        }

                        file.WriteLine("\t\tpublic AndroidJavaObject GetObj() => base.JavaObject;");
                        file.WriteLine("\t\tpublic void SetObj(AndroidJavaObject arg0) => base.JavaObject = arg0;");
                        file.WriteLine("\t}\n}");
                    }
                }

                AssetDatabase.Refresh();
            }
        }

        private string WriteProperty(Field field)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("\t\tpublic " + GetFieldType(field.fieldType) + " " + field.fieldName.ToPascalCase() + "\n\t\t{");
            switch (field.fieldType)
            {
                case "String":
                    builder.AppendLine("\t\t\tget { return Call<string>(\"get" + field.fieldName.ToPascalCase() + "\"); }");
                    builder.AppendLine("\t\t\tset { Call(\"set" + field.fieldName.ToPascalCase() + "\", value); }");
                    break;
                case "Boolean":
                    builder.AppendLine("\t\t\tget { return Call<bool>(\"get" + field.fieldName.ToPascalCase() + "\"); }");
                    builder.AppendLine("\t\t\tset { Call(\"set" + field.fieldName.ToPascalCase() + "\", value); }");
                    break;
                case "Short":
                    builder.AppendLine("\t\t\tget { return Call<AndroidJavaObject>(\"get" + field.fieldName.ToPascalCase() + "\").Call<short>(\"shortValue\"); }");
                    builder.AppendLine("\t\t\tset { Call(\"set" + field.fieldName.ToPascalCase() + "\", new AndroidJavaObject(\"java.lang.Short\", value)); }");
                    break;
                case "Byte":
                    builder.AppendLine("\t\t\tget { return Call<AndroidJavaObject>(\"get" + field.fieldName.ToPascalCase() + "\").Call<byte>(\"byteValue\"); }");
                    builder.AppendLine("\t\t\tset { Call(\"set" + field.fieldName.ToPascalCase() + "\", new AndroidJavaObject(\"java.lang.Byte\", value)); }");
                    break;
                case "Integer":
                    builder.AppendLine("\t\t\tget { return Call<AndroidJavaObject>(\"get" + field.fieldName.ToPascalCase() + "\").Call<int>(\"intValue\"); }");
                    builder.AppendLine("\t\t\tset { Call(\"set" + field.fieldName.ToPascalCase() + "\", new AndroidJavaObject(\"java.lang.Integer\", value)); }");
                    break;
                case "Long":
                    builder.AppendLine("\t\t\tget { return Call<AndroidJavaObject>(\"get" + field.fieldName.ToPascalCase() + "\").Call<long>(\"longValue\"); }");
                    builder.AppendLine("\t\t\tset { Call(\"set" + field.fieldName.ToPascalCase() + "\", new AndroidJavaObject(\"java.lang.Long\", value)); }");
                    break;
                case "Float":
                    builder.AppendLine("\t\t\tget { return Call<AndroidJavaObject>(\"get" + field.fieldName.ToPascalCase() + "\").Call<float>(\"floatValue\"); }");
                    builder.AppendLine("\t\t\tset { Call(\"set" + field.fieldName.ToPascalCase() + "\", new AndroidJavaObject(\"java.lang.Float\", value)); }");
                    break;
                case "Double":
                    builder.AppendLine("\t\t\tget { return Call<AndroidJavaObject>(\"get" + field.fieldName.ToPascalCase() + "\").Call<double>(\"doubleValue\"); }");
                    builder.AppendLine("\t\t\tset { Call(\"set" + field.fieldName.ToPascalCase() + "\", new AndroidJavaObject(\"java.lang.Double\", value)); }");
                    break;
                case "ByteArray":
                    builder.AppendLine("\t\t\tget { return Call<byte[]>(\"get" + field.fieldName.ToPascalCase() + "\"); }");
                    builder.AppendLine("\t\t\tset { Call(\"set" + field.fieldName.ToPascalCase() + "\", value); }");
                    break;
                case "Text":
                    builder.AppendLine("\t\t\tget { return Call<AndroidJavaObject>(\"get" + field.fieldName.ToPascalCase() + "\").Call<string>(\"get\"); }");
                    builder.AppendLine("\t\t\tset { Call(\"set" + field.fieldName.ToPascalCase() + "\", new AndroidJavaObject(\"com.huawei.agconnect.cloud.database.Text\", value)); }");
                    break;
                case "Date":
                    builder.AppendLine("\t\t\tget { return new DateTime(Call<AndroidJavaObject>(\"get" + field.fieldName.ToPascalCase() + "\").Call<long>(\"getTime\")); }");
                    builder.AppendLine("\t\t\tset { Call(\"set" + field.fieldName.ToPascalCase() + "\", new AndroidJavaObject(\"java.util.Date\", value.Ticks)); }");
                    break;
                default:
                    break;
            }
            builder.AppendLine("\t\t}");
            return builder.ToString();
        }

        private string GetFieldType(string fieldType)
        {
            switch (fieldType)
            {
                case "String":
                    return "string";
                case "Boolean":
                    return "bool";
                case "Short":
                    return "short";
                case "Byte":
                    return "byte";
                case "Integer":
                    return "int";
                case "Long":
                    return "long";
                case "Float":
                    return "float";
                case "Double":
                    return "double";
                case "ByteArray":
                    return "byte[]";
                case "Text":
                    return "string";
                case "Date":
                    return "DateTime";
                default:
                    break;
            }
            return "";
        }

        [System.Serializable]
        private class Permission
        {
            public string role;
            public List<string> rights;
        }
        [System.Serializable]
        private class Index
        {
            public string indexName;
            public List<string> indexList;
        }
        [System.Serializable]
        private class Field
        {
            public bool isNeedEncrypt;
            public string fieldName;
            public bool notNull;
            public bool belongPrimaryKey;
            public string fieldType;
            public string defaultValue;
        }
        [System.Serializable]
        private class ObjectType
        {
            public List<Index> indexes;
            public string objectTypeName;
            public List<Field> fields;
        }
        [System.Serializable]
        private class Root
        {
            public int schemaVersion;
            public List<PermissionRoot> permissions;
            public List<ObjectType> objectTypes;
        }
        [System.Serializable]
        private class PermissionRoot
        {
            public List<Permission> permissions;
            public string objectTypeName;
        }
    }
}
