/*
 * Created by SharpDevelop.
 * User: james
 * Date: 16/09/2022
 * Time: 8:42 p. m.
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.InteropServices;

namespace jxshell.dotnet4
{
	/// <summary>
	/// Description of DynamicValue.
	/// </summary>
	/// 
	[ComVisible(true)]
	public class DynamicValue: DynamicObject{
		
		internal IDictionary<string, object> dict;
		internal bool list = false;
		
		public DynamicValue():this(new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase)){
			
		}
		
		public DynamicValue(Dictionary<string, object> dict){
			this.dict = dict; 
		}
		
		public object innerDictionary{
			get{
				return dict;
			}
		}
		
		public object GetValue(string name){
			object result = null;
			var ok = this._TryGetMember(name, out result);
			if(!ok) return null;
			return result; 
		}
		
		public object GetValue(int index){
			return GetValue(index.ToString());
		}
		
		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			dict[binder.Name] = value; 
			return true; 
		}
		
		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			return _TryGetMember(binder.Name, out result);
		}
		
		
		
		
		public bool _TryGetMember(string name, out object result){			
			result = null;
			if(name != null ){
				
				if(list){
					if((name.ToUpper() == "COUNT") || (name.ToUpper() == "LENGTH")){
						result = dict.Count;
						return true; 
					}
						 
				}
				
				if(this.dict.ContainsKey(name)){
					result = this.dict[name];
					
					if(result is IDictionary<string, object>){
						var value = result as IDictionary<string, object>;
						if(!(value is DynamicValue)){
							if(value != null){
								result = new DynamicValue(new Dictionary<string, object>(value));
							}
						}	
					}
					
					
					if(result is List<Object>){
						var value = result as List<object>;
						var shideo = new DynamicValue();
						shideo.list = true; 
						var count = 0;
						foreach(var item in value){
							shideo.dict.Add(count.ToString(), item);
							count++;
						}
						result = shideo; 
					}				
					
				}
			}
			
			return true; 
		}		
	}
}
