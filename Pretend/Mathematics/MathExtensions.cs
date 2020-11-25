using System;
using System.Numerics;
using TKVector3 = OpenTK.Mathematics.Vector3;
using TKVector4 = OpenTK.Mathematics.Vector4;
using TKMatrix4 = OpenTK.Mathematics.Matrix4;

namespace Pretend.Mathematics
{
    public static class MathExtensions
    {
        public static TKVector3 ToTKVector3(this Vector3 vector)
        {
            return new TKVector3(vector.X, vector.Y, vector.Z);
        }

        public static TKVector4 ToTKVector4(this Vector4 vector)
        {
            return new TKVector4(vector.X, vector.Y, vector.Z, vector.Z);
        }

        public static TKMatrix4 ToTkMatrix4(this Matrix4x4 matrix)
        {
            return new TKMatrix4(
                matrix.M11, matrix.M12, matrix.M13, matrix.M14,
                matrix.M21, matrix.M22, matrix.M23, matrix.M24,
                matrix.M31, matrix.M32, matrix.M33, matrix.M34,
                matrix.M41, matrix.M42, matrix.M43, matrix.M44);
        }

        public static Quaternion ToQuaternian(this Vector3 vector)
        {
            var radianVector = (float) Math.PI * vector / 180f;
            return Quaternion.CreateFromYawPitchRoll(radianVector.Y, radianVector.X, radianVector.Z);
        }

        public static float ToRadians(this float degrees)
        {
            return (float) Math.PI * degrees / 180f;
        }
    }
}
