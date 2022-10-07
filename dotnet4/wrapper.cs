using System;
using System.Runtime.InteropServices;

namespace jxshell.dotnet4
{
	
	
	[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]  
	public class UnwrappedAttribute : System.Attribute  
	{  
		
		internal bool value = false; 
	    public UnwrappedAttribute(bool value)  
	    {  
	    	this.value = value; 
	    }  
	}


	[ComVisible(true)]
	public class wrapper : wrapperBase
	{
		public wrapper()
		{
		}

		public wrapper(object o)
		{
			if (o == null)
			{
				throw new Exception("No se puede crear un objeto contenedor a partir de null");
			}
			this.wrappedObject = o;
			this.wrappedType = o.GetType();
			this.typeD = typeDescriptor.loadFromType(o.GetType());
		}

        public virtual void dispose()
        {
			if ((wrappedObject != null) && (wrappedObject is IDisposable))
			{
				((IDisposable)wrappedObject).Dispose();
			}
			wrappedObject = null;
            wrappedType = null;
            typeD = null;
        }

		public wrapper(object o, typeDescriptor td)
		{
			this.wrappedObject = o;
			this.wrappedType = o.GetType();
			this.typeD = td;
		}

		public override object __getProperty(string property, params object[] args)
		{
			object property2 = this.typeD.getProperty(this.wrappedObject, property, args);
			if (property2 != null && property2.Equals(this.wrappedObject))
			{
				return this;
			}
			return wrapper.getFromObject(property2);
		}

		public override object __invokeMethod(string method, params object[] args)
		{
			object obj = this.typeD.invokeMethod(this.wrappedObject, method, args);
			if (obj != null && obj.Equals(this.wrappedObject))
			{
				return this;
			}
			return wrapper.getFromObject(obj);
		}

		public object __process(object o)
		{
			if (o != null && o.GetType() == this.wrappedType && o.Equals(this.wrappedObject))
			{
				return this;
			}
			return wrapper.getFromObject(o);
		}

		public override void __setProperty(string property, string value, params object[] args)
		{
			this.typeD.setProperty(this.wrappedObject, property, value, args);
		}

		public static wrapper createWrapper(object o)
		{
			jxshell.dotnet4.typeDescriptor typeDescriptor = jxshell.dotnet4.typeDescriptor.loadFromType(o.GetType());
			return typeDescriptor.compile().getWrapper(o);
		}

		public static wrapper createWrapper(object o, typeDescriptor td)
		{
			return td.compile().getWrapper(o);
		}

		public override bool Equals(object obj)
		{
			return this.wrappedObject.Equals(obj);
		}

		public static object getFromObject(object o)
		{
			
			if (o == null || o is DBNull)
			{
				return null;
			}
            if (o is wrapper)
            {
                return o;
            }
            if (o is long)
			{
				return (double)((long)o);
			}
			if (o.GetType().IsPrimitive || o is string)
			{
				return o;
			}
			
			var uw = Attribute.GetCustomAttribute(o.GetType(), typeof(UnwrappedAttribute));
			if(uw != null){
				return o; 
			}
				
			return wrapper.createWrapper(o);
		}

		public override int GetHashCode()
		{
			return this.wrappedObject.GetHashCode();
		}

		public override string ToString()
		{
			return this.wrappedObject.ToString();
		}
	}
}