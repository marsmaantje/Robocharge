using System;

namespace GXPEngine.Core
{
	public struct Vector2
	{
		public float x;
		public float y;
		
		public Vector2 (float x, float y)
		{
			this.x = x;
			this.y = y;
		}
		
		public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(a.x + b.x, a.y + b.y);
		public static Vector2 operator -(Vector2 a, Vector2 b) => new Vector2(a.x - b.x, a.y - b.y);
		public static Vector2 operator *(Vector2 a, Vector2 b) => new Vector2(a.x * b.x, a.y * b.y);
		public static Vector2 operator /(Vector2 a, Vector2 b) => new Vector2(a.x / b.x, a.y / b.y);
		public static Vector2 operator /(Vector2 a, float b) => new Vector2(a.x / b, a.y / b);

		public void normalize()
        {
			this /= this.length();
        }

		public float length()
        {
			return Mathf.Sqrt((this.x * this.x) + (this.y * this.y));
		}

		override public string ToString() {
			return "[Vector2 " + x + ", " + y + "]";
		}
	}
}

