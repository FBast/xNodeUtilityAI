using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Plugins.xNodeUtilityAi.Utils;

namespace Plugins.xNodeUtilityAi.Framework {
    [Serializable]
    public class SerializableMemberInfo {

        public string DeclaringTypeName;
        public string Name;
        public string PortName;
        public string TypeName;
        public bool IsPrimitive;
        public bool IsIteratable;
        public int Order;

        public SerializableMemberInfo(MemberInfo memberInfo) {
            DeclaringTypeName = memberInfo.DeclaringType?.AssemblyQualifiedName;
            Name = memberInfo.Name;
            PortName = memberInfo.Name + " (" + memberInfo.MemberType + ")";
            Type fieldType = memberInfo.FieldType();
            TypeName = fieldType.AssemblyQualifiedName;
            IsPrimitive = fieldType.IsPrimitive;
            if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>)) {
                IsIteratable = true;
            }
            Order = memberInfo.MetadataToken;
        }

        public MemberInfo ToMemberInfo() {
            Type declaringType = Type.GetType(DeclaringTypeName);
            if (declaringType == null) throw new Exception("Cannot find declaring type : " + DeclaringTypeName);
            return declaringType.GetMember(Name).FirstOrDefault();
        }

        public object GetEditorValue() {
            MemberInfo memberInfo = ToMemberInfo();
            return IsPrimitive ? null : 
                new Tuple<string, Type, object>(Name, memberInfo.FieldType(), null);
        }

        public object GetRuntimeValue(object context) {
            MemberInfo memberInfo = ToMemberInfo();
            if (context == null) throw new Exception("Cannot get Runtime value if context is null");
            return IsPrimitive ? memberInfo.GetValue(context) : 
                new Tuple<string, Type, object>(Name, memberInfo.FieldType(), memberInfo.GetValue(context));
        }

    }
}