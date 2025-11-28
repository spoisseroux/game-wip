using UnityEngine;

namespace FIMSpace
{
    public class FImp_ColliderData_CharacterCapsule : FImp_ColliderData_Base
    {
        public CharacterController Capsule { get; private set; }
        private Vector3 Top;
        private Vector3 Bottom;
        private Vector3 Direction;
        private float radius;
        private float scaleFactor;
        private float preRadius;

        public FImp_ColliderData_CharacterCapsule(CharacterController collider)
        {
            Is2D = false;
            Transform = collider.transform;
            Collider = collider;
            Transform = collider.transform;
            Capsule = collider;
            ColliderType = EFColliderType.Capsule;
            CalculateCapsuleParameters(Capsule, ref Direction, ref radius, ref scaleFactor);
            RefreshColliderData();
        }

        public override void RefreshColliderData()
        {
            if (IsStatic) return; // No need to refresh collider data if it is static

            bool diff = false;

            if (!FEngineering.VIsSame(previousPosition, Transform.position)) diff = true;
            else
                if (!FEngineering.QIsSame(Transform.rotation, previousRotation)) diff = true;
            else
            {
                if (preRadius != Capsule.radius || !FEngineering.VIsSame(previousScale, Transform.lossyScale))
                    CalculateCapsuleParameters(Capsule, ref Direction, ref radius, ref scaleFactor);
            }

            if (diff)
            {
                GetCapsuleHeadsPositions(Capsule, ref Top, ref Bottom, Direction, radius, scaleFactor);
            }

            base.RefreshColliderData();

            previousPosition = Transform.position;
            previousRotation = Transform.rotation;
            previousScale = Transform.lossyScale;

            preRadius = Capsule.radius;
        }

        public override bool PushIfInside(ref Vector3 point, float pointRadius, Vector3 pointOffset)
        {
            return PushOutFromCapsuleCollider(pointRadius, ref point, Top, Bottom, radius, pointOffset, false);
        }

        public static bool PushOutFromCapsuleCollider(float segmentColliderRadius, ref Vector3 segmentPos, Vector3 capSphereCenter1, Vector3 capSphereCenter2, float capsuleRadius, Vector3 segmentOffset, bool is2D = false)
        {
            float radius = capsuleRadius + segmentColliderRadius;
            Vector3 capsuleUp = capSphereCenter2 - capSphereCenter1;
            Vector3 fromCenter = (segmentPos + segmentOffset) - capSphereCenter1;

            float orientationDot = Vector3.Dot(fromCenter, capsuleUp);

            if (orientationDot <= 0) // Main Sphere Cap
            {
                float sphereRefDistMagn = fromCenter.sqrMagnitude;

                if (sphereRefDistMagn > 0 && sphereRefDistMagn < radius * radius)
                {
                    segmentPos = capSphereCenter1 - segmentOffset + fromCenter * (radius / Mathf.Sqrt(sphereRefDistMagn));
                    return true;
                }
            }
            else
            {
                float upRefMagn = capsuleUp.sqrMagnitude;
                if (orientationDot >= upRefMagn) // Counter Sphere Cap
                {
                    fromCenter = (segmentPos + segmentOffset) - capSphereCenter2;
                    float sphereRefDistMagn = fromCenter.sqrMagnitude;

                    if (sphereRefDistMagn > 0 && sphereRefDistMagn < radius * radius)
                    {
                        segmentPos = capSphereCenter2 - segmentOffset + fromCenter * (radius / Mathf.Sqrt(sphereRefDistMagn));
                        return true;
                    }
                }
                else if (upRefMagn > 0) // Cylinder Volume
                {
                    fromCenter -= capsuleUp * (orientationDot / upRefMagn);
                    float sphericalRefDistMagn = fromCenter.sqrMagnitude;

                    if (sphericalRefDistMagn > 0 && sphericalRefDistMagn < radius * radius)
                    {
                        float projectedDistance = Mathf.Sqrt(sphericalRefDistMagn);
                        segmentPos += fromCenter * ((radius - projectedDistance) / projectedDistance);
                        return true;
                    }
                }
            }

            return false;
        }



        #region Capsule Calculations Helpers

        /// <summary>
        /// Calculating capsule's centers of up and down sphere which are fitting unity capsule collider with all collider's transformations
        /// </summary>
        protected static void CalculateCapsuleParameters(CharacterController capsule, ref Vector3 direction, ref float trueRadius, ref float scalerFactor)
        {
            Transform cTransform = capsule.transform;

            float radiusScaler;

            direction = Vector3.up; scalerFactor = cTransform.lossyScale.y;
            radiusScaler = cTransform.lossyScale.x > cTransform.lossyScale.z ? cTransform.lossyScale.x : cTransform.lossyScale.z;

            trueRadius = capsule.radius * radiusScaler;
        }

        protected static void GetCapsuleHeadsPositions(CharacterController capsule, ref Vector3 upper, ref Vector3 bottom, Vector3 direction, float radius, float scalerFactor)
        {
            Vector3 upCapCenter = direction * ((capsule.height / 2) * scalerFactor - radius); // Local Space Position
            upper = capsule.transform.position + capsule.transform.TransformDirection(upCapCenter) + capsule.transform.TransformVector(capsule.center); // World Space

            Vector3 downCapCenter = -direction * ((capsule.height / 2) * scalerFactor - radius);
            bottom = capsule.transform.position + capsule.transform.TransformDirection(downCapCenter) + capsule.transform.TransformVector(capsule.center);
        }

        #endregion

    }
}
