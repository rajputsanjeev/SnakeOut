using System;

public static class TupleExtensions{
		public static (float x,float y,float z,float w) SetValue(this (float x,float y,float z,float w) current,float value){
			return (value,value,value,value);
		}
		//===================
		// Vector3
		//===================
		public static (float x,float y,float z) SetValue(this (float x,float y,float z) current,float value){
			return (value,value,value);
		}
		public static (float x,float y,float z) AddAmount(this (float x,float y,float z) current,(float x,float y,float z) valueB){
			return (current.x+valueB.x,current.y+valueB.y,current.z+valueB.z);
		}
		public static (float x,float y,float z) Subtract(this (float x,float y,float z) current,(float x,float y,float z) valueB){
			return (current.x-valueB.x,current.y-valueB.y,current.z-valueB.z);
		}
		public static (float x,float y,float z) Multiply(this (float x,float y,float z) current,(float x,float y,float z) valueB){
			return (current.x*valueB.x,current.y*valueB.y,current.z*valueB.z);
		}
		public static (float x,float y,float z) Divide(this (float x,float y,float z) current,(float x,float y,float z) valueB){
			return (current.x/valueB.x,current.y/valueB.y,current.z/valueB.z);
		}
		public static (float x,float y,float z) Divide(this (float x,float y,float z) current,(int x,int y,int z) valueB){
			return (current.x/valueB.x,current.y/valueB.y,current.z/valueB.z);
		}
		public static (float x,float y,float z) Divide(this (float x,float y,float z) current,float valueB){
			return (current.x/valueB,current.y/valueB,current.z/valueB);
		}
		public static (float x,float y,float z) Divide(this (float x,float y,float z) current,int valueB){
			return (current.x/valueB,current.y/valueB,current.z/valueB);
		}
		public static (float x,float y,float z) Modulo(this (float x,float y,float z) current,(float x,float y,float z) valueB){
			return (current.x%valueB.x,current.y%valueB.y,current.z%valueB.z);
		}
		public static (float x,float y,float z) Scale(this (float x,float y,float z) current,float multiplier){
			return (current.x*multiplier,current.y*multiplier,current.z*multiplier);
		}
		public static (float x,float y,float z) Negate(this (float x,float y,float z) current){
			return (-current.x,-current.y,-current.z);
		}
		public static (float x,float y,float z) Clamp(this (float x,float y,float z) current,(float x,float y,float z) minimum,(float x,float y,float z) maximum){
			current.x = current.x.Clamp(minimum.x,maximum.x);
			current.y = current.y.Clamp(minimum.y,maximum.y);
			current.z = current.z.Clamp(minimum.z,maximum.z);
			return current;
		}
		public static (float x,float y,float z) MoveTowards(this (float x,float y,float z) current,(float x,float y,float z) target,(float x,float y,float z) by){
			current.x = current.x.MoveTowards(target.x,by.x);
			current.y = current.y.MoveTowards(target.y,by.y);
			current.z = current.z.MoveTowards(target.z,by.z);
			return current;
		}
		public static bool Within(this (float x,float y,float z) current,(float x,float y,float z) minimum,(float x,float y,float z) maximum){
			if(current.x < minimum.x || current.x > maximum.x){return false;}
			if(current.y < minimum.y || current.y > maximum.y){return false;}
			if(current.z < minimum.z || current.z > maximum.z){return false;}
			return true;
		}
		public static bool Equals(this (float x,float y,float z) current,(float x,float y,float z) valueB){
			if(current.x != valueB.x){return false;}
			if(current.y != valueB.y){return false;}
			if(current.z != valueB.z){return false;}
			return true;
		}

		public static float Dot(this (float x,float y,float z) current,(float x,float y,float z) other){
			return current.x * other.x + current.y * other.y + current.z * other.z;
		}
		public static (float x,float y,float z) Cross(this (float x,float y,float z) current,(float x,float y,float z) other){
			var x = current.y * other.z - other.y * current.z;
			var y = current.z * other.x - other.z * current.x;
			var z = current.x * other.y - other.x * current.y;
			return (x,y,z);
		}
		public static (float x,float y,float z,float w) AddDimension(this (float x,float y,float z) current){
			return (current.x,current.y,current.z,0);
		}
		public static (int x,int y,int z) AddAmount(this (int x,int y,int z) current,(int x,int y,int z) valueB){
			return (current.x+valueB.x,current.y+valueB.y,current.z+valueB.z);
		}
		public static (int x,int y,int z) Subtract(this (int x,int y,int z) current,(int x,int y,int z) valueB){
			return (current.x-valueB.x,current.y-valueB.y,current.z-valueB.z);
		}
		public static (int x,int y,int z) Multiply(this (int x,int y,int z) current,(int x,int y,int z) valueB){
			return (current.x*valueB.x,current.y*valueB.y,current.z*valueB.z);
		}
		public static (int x,int y,int z) Divide(this (int x,int y,int z) current,(int x,int y,int z) valueB){
			return (current.x/valueB.x,current.y/valueB.y,current.z/valueB.z);
		}
		public static (int x,int y,int z) Divide(this (int x,int y,int z) current,int valueB){
			return (current.x/valueB,current.y/valueB,current.z/valueB);
		}
		public static (int x,int y,int z) Modulo(this (int x,int y,int z) current,(int x,int y,int z) valueB){
			return (current.x%valueB.x,current.y%valueB.y,current.z%valueB.z);
		}
		public static (int x,int y,int z) Scale(this (int x,int y,int z) current,int multiplier){
			return (current.x*multiplier,current.y*multiplier,current.z*multiplier);
		}
		public static (int x,int y,int z) Negate(this (int x,int y,int z) current){
			return (-current.x,-current.y,-current.z);
		}
		public static (int x,int y,int z) Clamp(this (int x,int y,int z) current,(int x,int y,int z) minimum,(int x,int y,int z) maximum){
			current.x = current.x.Clamp(minimum.x,maximum.x);
			current.y = current.y.Clamp(minimum.y,maximum.y);
			current.z = current.z.Clamp(minimum.z,maximum.z);
			return current;
		}
		public static (int x,int y,int z) MoveTowards(this (int x,int y,int z) current,(int x,int y,int z) target,(int x,int y,int z) by){
			current.x = current.x.MoveTowards(target.x,by.x);
			current.y = current.y.MoveTowards(target.y,by.y);
			current.z = current.z.MoveTowards(target.z,by.z);
			return current;
		}
		public static bool Within(this (int x,int y,int z) current,(int x,int y,int z) minimum,(int x,int y,int z) maximum){
			if(current.x < minimum.x || current.x > maximum.x){return false;}
			if(current.y < minimum.y || current.y > maximum.y){return false;}
			if(current.z < minimum.z || current.z > maximum.z){return false;}
			return true;
		}
		public static bool Equals(this (int x,int y,int z) current,(int x,int y,int z) valueB){
			if(current.x != valueB.x){return false;}
			if(current.y != valueB.y){return false;}
			if(current.z != valueB.z){return false;}
			return true;
		}
		//===================
		// Vector2
		//===================
		public static (float x,float y) SetValue(this (float x,float y) current,float value){
			return (value,value);
		}
		public static float Length(this (float x,float y) current){
			return (float)Math.Sqrt(current.x * current.x + current.y * current.y);
		}
		public static float LengthFast(this (float x,float y) current){
			var length = current.x * current.x + current.y * current.y;
			length = 1/length.InverseSquare();
			return length;
		}
		public static float LengthSquare(this (float x,float y) current){
			return (float)(current.x * current.x + current.y * current.y);
		}
		public static float Distance(this (float x,float y) current,(float x,float y) valueB){
			return valueB.Subtract(current).Length().Abs();
		}
		public static float DistanceFast(this (float x,float y) current,(float x,float y) valueB){
			return valueB.Subtract(current).LengthFast().Abs();
		}
		public static (float x,float y) Normalize(this (float x,float y) current){
			var length = current.Length();
			return length > 0 ? (current.x/length,current.y/length) : current;
		}
		public static (float x,float y) NormalizeFast(this (float x,float y) current){
			var length = current.x * current.x + current.y * current.y;
			length = 1/length.InverseSquare();
			return length > 0 ? (current.x/length,current.y/length) : current;
		}
		public static (float x,float y) AddAmount(this (float x,float y) current,(float x,float y) valueB){
			return (current.x+valueB.x,current.y+valueB.y);
		}
		public static (float x,float y) Subtract(this (float x,float y) current,float valueB){
			return (current.x-valueB,current.y-valueB);
		}
		public static (float x,float y) Subtract(this (float x,float y) current,(float x,float y) valueB){
			return (current.x-valueB.x,current.y-valueB.y);
		}
		public static (float x,float y) Multiply(this (float x,float y) current,float valueB){
			return (current.x*valueB,current.y*valueB);
		}
		public static (float x,float y) Multiply(this (float x,float y) current,(float x,float y) valueB){
			return (current.x*valueB.x,current.y*valueB.y);
		}
		public static (float x,float y) Divide(this (float x,float y) current,(float x,float y) valueB){
			return (current.x/valueB.x,current.y/valueB.y);
		}
		public static (float x,float y) Divide(this (float x,float y) current,(int x,int y) valueB){
			return (current.x/valueB.x,current.y/valueB.y);
		}
		public static (float x,float y) Divide(this (float x,float y) current,float valueB){
			return (current.x/valueB,current.y/valueB);
		}
		public static (float x,float y) Divide(this (float x,float y) current,int valueB){
			return (current.x/valueB,current.y/valueB);
		}
		public static (float x,float y) Modulo(this (float x,float y) current,(float x,float y) valueB){
			return (current.x%valueB.x,current.y%valueB.y);
		}
		public static (float x,float y) Scale(this (float x,float y) current,float multiplier){
			return (current.x*multiplier,current.y*multiplier);
		}
		public static (float x,float y) Negate(this (float x,float y) current){
			return (-current.x,-current.y);
		}
		public static (float x,float y) Clamp(this (float x,float y) current,(float x,float y) minimum,(float x,float y) maximum){
			current.x = current.x.Clamp(minimum.x,maximum.x);
			current.y = current.y.Clamp(minimum.y,maximum.y);
			return current;
		}
		public static (float x,float y) MoveTowards(this (float x,float y) current,(float x,float y) target,(float x,float y) by){
			current.x = current.x.MoveTowards(target.x,by.x);
			current.y = current.y.MoveTowards(target.y,by.y);
			return current;
		}
		public static (float x,float y) Lerp(this (float x,float y) current,(float x,float y) target,float time){
			if(time <= 0){time=0;}
			if(time >= 1){time=1;}
			return(
				current.x + (target.x - current.x) * time,
				current.y + (target.y - current.y) * time
			);
		}
		public static (int x,int y) Lerp(this (int x,int y) current,(int x,int y) target,float time){
			if(time <= 0){time=0;}
			if(time >= 1){time=1;}
			return(
				(int)(current.x + (target.x - current.x) * time),
				(int)(current.y + (target.y - current.y) * time)
			);
		}
		public static bool Within(this (float x,float y) current,(float x,float y) minimum,(float x,float y) maximum){
			if(current.x < minimum.x || current.x > maximum.x){return false;}
			if(current.y < minimum.y || current.y > maximum.y){return false;}
			return true;
		}
		public static bool Equals(this (float x,float y) current,(float x,float y) valueB){
			if(current.x != valueB.x){return false;}
			if(current.y != valueB.y){return false;}
			return true;
		}
		public static int Length(this (int x,int y) current){
			return (int)Math.Sqrt(current.x * current.x + current.y * current.y);
		}
		public static int LengthFast(this (int x,int y) current){
			var length = current.x * current.x + current.y * current.y;
			length = (int)Math.Round(1/length.InverseSquare());
			return length;
		}
		public static int LengthSquare(this (int x,int y) current){
			return (int)(current.x * current.x + current.y * current.y);
		}
		public static int Distance(this (int x,int y) current,(int x,int y) valueB){
			return valueB.Subtract(current).Length().Abs();
		}
		public static int DistanceFast(this (int x,int y) current,(int x,int y) valueB){
			return valueB.Subtract(current).LengthFast().Abs();
		}
		public static (int x,int y) Normalize(this (int x,int y) current){
			var length = current.Length();
			return length > 0 ? (current.x/length,current.y/length) : current;
		}
		public static (int x,int y) NormalizeFast(this (int x,int y) current){
			var length = current.x * current.x + current.y * current.y;
			length = (int)Math.Round(1/length.InverseSquare());
			return length > 0 ? (current.x/length,current.y/length) : current;
		}
		public static (int x,int y) AddAmount(this (int x,int y) current,(int x,int y) valueB){
			return (current.x+valueB.x,current.y+valueB.y);
		}
		public static (float x,float y) AddAmount(this (int x,int y) current,(float x,float y) valueB){
			return (current.x+valueB.x,current.y+valueB.y);
		}
		public static (int x,int y) Subtract(this (int x,int y) current,(int x,int y) valueB){
			return (current.x-valueB.x,current.y-valueB.y);
		}
		public static (int x,int y) Subtract(this (int x,int y) current,int valueB){
			return (current.x-valueB,current.y-valueB);
		}
		public static (float x,float y) Subtract(this (int x,int y) current,float valueB){
			return (current.x-valueB,current.y-valueB);
		}
		public static (float x,float y) Subtract(this (int x,int y) current,(float x,float y) valueB){
			return (current.x-valueB.x,current.y-valueB.y);
		}
		public static (int x,int y) Multiply(this (int x,int y) current,(int x,int y) valueB){
			return (current.x*valueB.x,current.y*valueB.y);
		}
		public static (int x,int y) Multiply(this (int x,int y) current,int valueB){
			return (current.x*valueB,current.y*valueB);
		}
		public static (int x,int y) Multiply(this (int x,int y) current,float valueB){
			return ((int)(current.x*valueB),(int)(current.y*valueB));
		}
		public static (int x,int y) Divide(this (int x,int y) current,(int x,int y) valueB){
			return (current.x/valueB.x,current.y/valueB.y);
		}
		public static (int x,int y) Divide(this (int x,int y) current,int valueB){
			return (current.x/valueB,current.y/valueB);
		}
		public static (int x,int y) Modulo(this (int x,int y) current,(int x,int y) valueB){
			return (current.x%valueB.x,current.y%valueB.y);
		}
		public static (int x,int y) Scale(this (int x,int y) current,int multiplier){
			return (current.x*multiplier,current.y*multiplier);
		}
		public static (float x,float y) Scale(this (int x,int y) current,float multiplier){
			return (current.x*multiplier,current.y*multiplier);
		}
		public static (int x,int y) Negate(this (int x,int y) current){
			return (-current.x,-current.y);
		}
		public static (int x,int y) Clamp(this (int x,int y) current,(int x,int y) minimum,(int x,int y) maximum){
			current.x = current.x.Clamp(minimum.x,maximum.x);
			current.y = current.y.Clamp(minimum.y,maximum.y);
			return current;
		}
		public static (int x,int y) MoveTowards(this (int x,int y) current,(int x,int y) target,(int x,int y) by){
			current.x = current.x.MoveTowards(target.x,by.x);
			current.y = current.y.MoveTowards(target.y,by.y);
			return current;
		}
		public static bool Within(this (int x,int y) current,(int x,int y) minimum,(int x,int y) maximum){
			if(current.x < minimum.x || current.x > maximum.x){return false;}
			if(current.y < minimum.y || current.y > maximum.y){return false;}
			return true;
		}
		public static bool Equals(this (int x,int y) current,(int x,int y) valueB){
			if(current.x != valueB.x){return false;}
			if(current.y != valueB.y){return false;}
			return true;
		}
		//===================
		// Others
		//===================
		public static (float x,float y) ToFloat(this (int x,int y) current){
			return (current.x,current.y);
		}
		public static (int x,int y) ToInt(this (float x,float y) current){
			return ((int)current.x,(int)current.y);
		}
		public static float Pack(this (float a,float b,float c,float d) current){
			var x = ((int)Math.Floor(current.a * 63))<<18;
			var y = ((int)Math.Floor(current.b * 63))<<12;
			var z = ((int)Math.Floor(current.c * 63))<<6;
			var w = ((int)Math.Floor(current.d * 63));
			return (x | y | z | w) * 0.0000001f;
		}
	}