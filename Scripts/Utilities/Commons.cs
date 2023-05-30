using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.IO;
using System.Linq;
using RandomNumberGeneration;

namespace Utilities
{
    public static class Commons
    {
        private static List<int> triangularSteps = new List<int>();

        #region Vector Calculation -----------------------------------------------------------------
        /// <summary>
        /// Calculates the mean betweean all given vectors.
        /// by someone of Stackoverflow. I cant find the source anymore
        /// </summary>
        /// <param name="_positions">The vectors</param>
        /// <returns>The mean vector</returns>
        public static Vector3 MeanVector(List<Vector3> _positions)
        {
            if (_positions.Count == 0)
                return Vector3.zero;
            float x = 0f;
            float y = 0f;
            float z = 0f;
            foreach (Vector3 pos in _positions)
            {
                x += pos.x;
                y += pos.y;
                z += pos.z;
            }
            return new Vector3(x / _positions.Count, y / _positions.Count, z / _positions.Count);
        }

        /// <summary>
        /// Calculates the perpendicular vector to a vector counterclockwise.
        /// </summary>
        /// <param name="_vector">The vector to calculate the perpendicular to</param>
        /// <returns>The perpendicular vector</returns>
        public static Vector3 PerpendicularCounterClockwise(Vector3 _vector) => new Vector3(-_vector.z, 0, _vector.x);

        /// <summary>
        /// Calculates the perpendicular vector to a vector clockwise.
        /// </summary>
        /// <param name="_vector">The vector to calculate the perpendicular to</param>
        /// <returns>The perpendicular vector</returns>
        public static Vector3 PerpendicularClockwise(Vector3 _vector) => new Vector3(_vector.z, 0, -_vector.x);

        /// <summary>
        /// Maps a Vector2 into a 3D space plane without height. 
        /// 
        /// in.x => out.x
        /// in.y => out.z
        /// </summary>
        /// <param name="_input">Vector to use for mapping</param>
        /// <returns>Mapped Vector3</returns>
        public static Vector3 MapToFlatPlane(this Vector2 _input) => new Vector3(_input.x, 0, _input.y);

        /// <summary>
        /// Maps a Vector3 into a 2D space. 
        /// 
        /// in.x => out.x
        /// in.z => out.y
        /// </summary>
        /// <param name="_input">Vector to use for mapping</param>
        /// <returns>Mapped Vector2</returns>
        public static Vector2 MapPlaneToVector2(this Vector3 _input) => new Vector2(_input.x, _input.z);

        /// <summary>
        /// Maps a Vector3 reversed into a 2D space. 
        /// 
        /// in.x => out.y
        /// in.z => out.x
        /// </summary>
        /// <param name="_input">Vector to use for mapping</param>
        /// <returns>Mapped Vector2</returns>
        public static Vector2 MapPlaneToVector2Reverse(this Vector3 _input) => new Vector2(_input.z, _input.x);

        /// <summary>
        /// Clamps a vector3 by some bounds.
        /// </summary>
        /// <param name="_value">Value to clamp</param>
        /// <param name="_xMin">x Min</param>
        /// <param name="_xMax">x Max</param>
        /// <param name="_yMin">y Min</param>
        /// <param name="_yMax">y Max</param>
        /// <param name="_zMin">z Max</param>
        /// <param name="_zMax">z Min</param>
        /// <returns>Clamped Vector3</returns>
        public static Vector3 Clamp(Vector3 _value, float _xMin, float _xMax, float _yMin, float _yMax, float _zMin, float _zMax)
        {
            return new Vector3(Mathf.Clamp(_value.x, _xMin, _xMax), Mathf.Clamp(_value.y, _yMin, _yMax), Mathf.Clamp(_value.z, _zMin, _zMax));
        }

        public static Vector3 RemoveY(this Vector3 _vector) => new Vector3(_vector.x, 0, _vector.z);

        public static Vector3 OnlyY(this Vector3 _vector) => new Vector3(0, _vector.y, 0);

        public static Vector3 InverseScaleVector(Vector3 _scaleVector)
        {
            return new Vector3(1 - _scaleVector.x + 1, 1 - _scaleVector.y + 1, 1 - _scaleVector.z + 1);
        }

        #endregion -----------------------------------------------------------------

        #region Layering -----------------------------------------------------------------
        /// <summary>
        /// Checks if a layer is within a layermask.
        /// by https://answers.unity.com/questions/50279/check-if-layer-is-in-layermask.html
        /// </summary>
        /// <param name="_mask">The layermask</param>
        /// <param name="_layer">The layer</param>
        /// <returns>True if it is in the layer</returns>
        public static bool IsInLayerMask(this int _layer, LayerMask _mask) => _mask == (_mask | (1 << _layer));
        #endregion -----------------------------------------------------------------

        #region Falloff -----------------------------------------------------------------
        /// <summary>
        /// Applies a falloff to a value.
        /// The higher the value in applicationVal the lower val gets.
        /// Values should be 0 to 1.
        /// </summary>
        /// <param name="_val">Value that should be changed</param>
        /// <param name="_applicationVal">Value for application</param>
        /// <returns>Applied falloff value</returns>
        public static float FallOff(float _val, float _applicationVal) => _val * Mathf.Clamp01((_applicationVal * -1) + 1);

        /// <summary>
        /// Applies a falloff to a value.
        /// The lower the value in applicationVal the lower val gets.
        /// Values should be 0 to 1.
        /// </summary>
        /// <param name="_val">Value that should be changed</param>
        /// <param name="_applicationVal">Value for application</param>
        /// <returns>Applied falloff value</returns>
        public static float InvertedFallOff(float _val, float _applicationVal) => ((_val * Mathf.Clamp01(_applicationVal)) + (_val + Mathf.Clamp01(_applicationVal)) * 0.5f) / 2;

        #endregion -----------------------------------------------------------------

        #region Inverting -----------------------------------------------------------------
        public static float Invert(float _val, float _maxValue) => _maxValue - _val;

        #endregion -----------------------------------------------------------------

        #region Assigning Eventtrigger Listeners -----------------------------------------------------------------
        /// <summary>
        /// Assigns a pointer click listener to an EventTrigger with a specified callback.
        /// </summary>
        /// <param name="_trigger">Trigger to assign the listener to</param>
        /// <param name="_method">Callback method</param>
        public static void AssignPointerClickListener(EventTrigger _trigger, UnityAction<BaseEventData> _method)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener(_method);
            _trigger.triggers.Add(entry);
        }

        /// <summary>
        /// Assigns a pointer enter listener to an EventTrigger with a specified callback.
        /// </summary>
        /// <param name="_trigger">Trigger to assign the listener to</param>
        /// <param name="_method">Callback method</param>
        public static void AssignPointerEnterListener(EventTrigger _trigger, UnityAction<BaseEventData> _method)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener(_method);
            _trigger.triggers.Add(entry);
        }

        /// <summary>
        /// Assigns a pointer exit listener to an EventTrigger with a specified callback.
        /// </summary>
        /// <param name="_trigger">Trigger to assign the listener to</param>
        /// <param name="_method">Callback method</param>
        public static void AssignPointerExitListener(EventTrigger _trigger, UnityAction<BaseEventData> _method)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerExit;
            entry.callback.AddListener(_method);
            _trigger.triggers.Add(entry);
        }
        #endregion -----------------------------------------------------------------

        #region Triangular Steps -----------------------------------------------------------------
        /// <summary>
        /// Calculates the traingular step for the parameter.
        /// </summary>
        /// <returns>Step of triangular list</returns>
        public static int TriangularStep(int _val)
        {
            if (_val <= 1)
                return 1;

            int step = 0;
            do
            {
                while (step + 1 >= triangularSteps.Count)
                    triangularSteps.Add(KleinerGauss(step + 1));
            }
            while (triangularSteps[step++] < _val);

            return step - 1;
        }

        /// <summary>
        /// Calculates the traingular step for the parameter.
        /// </summary>
        /// <returns>Step of triangular list</returns>
        public static int TriangularStepBoundValue(int _step)
        {
            for (int i = triangularSteps.Count; i <= _step; i++)
            {
                if (i >= triangularSteps.Count)
                    triangularSteps.Add(KleinerGauss(i));
            }

            return triangularSteps[_step];
        }



        #endregion -----------------------------------------------------------------

        #region Direction Links -----------------------------------------------------------------
        public static E2DLinkNonDiagonal ReverseDirection(E2DLinkNonDiagonal _direction)
        {
            switch (_direction)
            {
                case E2DLinkNonDiagonal.North:
                    return E2DLinkNonDiagonal.South;
                case E2DLinkNonDiagonal.East:
                    return E2DLinkNonDiagonal.West;
                case E2DLinkNonDiagonal.South:
                    return E2DLinkNonDiagonal.North;
                case E2DLinkNonDiagonal.West:
                    return E2DLinkNonDiagonal.East;
                default:
                    throw new ArgumentException();
            }
        }

        public static E3DLink GetLink(int _xFrom, int _zFrom, int _yFrom, int _xTo, int _zTo, int _yTo)
        {
            string linkString = "";

            if (_xFrom == _xTo + 1 && _zFrom == _zTo - 1)
                linkString = "NorthWest";

            else if (_xFrom == _xTo && _zFrom == _zTo - 1)
                linkString = "North";

            else if (_xFrom == _xTo - 1 && _zFrom == _zTo - 1)
                linkString = "NorthEast";

            else if (_xFrom == _xTo - 1 && _zFrom == _zTo)
                linkString = "East";

            else if (_xFrom == _xTo - 1 && _zFrom == _zTo + 1)
                linkString = "SouthEast";

            else if (_xFrom == _xTo && _zFrom == _zTo + 1)
                linkString = "South";

            else if (_xFrom == _xTo + 1 && _zFrom == _zTo + 1)
                linkString = "SouthWest";

            else if (_xFrom == _xTo + 1 && _zFrom == _zTo)
                linkString = "West";

            if (_yFrom == _yTo - 1)
                linkString += "Top";

            else if (_yFrom == _yTo + 1)
                linkString += "Bottom";

            if (Enum.TryParse<E3DLink>(linkString, out E3DLink link))
                return link;
            else
                return E3DLink.None;
        }

        /// <summary>
        /// NOT FULLY TESTED!!!
        /// <summary>
        public static bool GetCellIndexByLink2D(int _x, int _z, 
            int _textureXSize, int _textureZSize, 
            E3DLink _link, out Vector2 coords)
        {
            coords = new Vector2(_x, _z);
            switch (_link)
            {
                case E3DLink.North:
                    if (_textureZSize == _z + 1)
                        return false;

                    coords.y = _z + 1;
                    return true;
                case E3DLink.NorthEast:
                    if (_textureXSize == _x + 1)
                        return false;
                    if (_textureZSize == _z + 1)
                        return false;

                    coords.x = _x + 1;
                    coords.y = _z + 1;
                    return true;
                case E3DLink.East:
                    if (_textureXSize == _x + 1)
                        return false;

                    coords.x = _x + 1;
                    return true;
                case E3DLink.SouthEast:
                    if (_textureXSize == _x + 1)
                        return false;
                    if (_z - 1 < 0)
                        return false;

                    coords.x = _x + 1;
                    coords.y = _z - 1;
                    return true;
                case E3DLink.South:
                    if (_z - 1 < 0)
                        return false;

                    coords.y = _z - 1;
                    return true;
                case E3DLink.SouthWest:
                    if (_x - 1 < 0)
                        return false;
                    if (_z - 1 < 0)
                        return false;

                    coords.x = _x - 1;
                    coords.y = _z - 1;
                    return true;
                case E3DLink.West:
                    if (_x - 1 < 0)
                        return false;

                    coords.x = _x - 1;
                    return true;
                case E3DLink.NorthWest:
                    if (_x - 1 < 0)
                        return false;
                    if (_textureXSize == _z + 1)
                        return false;

                    coords.x = _x - 1;
                    coords.y = _z + 1;
                    return true;
                case E3DLink.None:
                    return false;
            }
            return false;
        }

        /// <summary>
        /// NOT FULLY TESTED!!!
        /// <summary>
        public static bool GetCellIndexByLink(int _x, int _z, int _y, 
            int _textureXSize, int _textureZSize, int _textureYSize,
            E3DLink _link, out Vector3 coords)
        {
            coords = new Vector3(_x, _y, _z);
            switch (_link)
            {
                case E3DLink.Bottom:
                    if (_y == 0)
                        return false;

                    coords.y = _y - 1;
                    return true;
                case E3DLink.NorthBottom:
                    if (_y == 0)
                        return false;
                    if (_textureZSize == _z + 1)
                        return false;

                    coords.y = _y - 1;
                    coords.z = _z + 1;
                    return true;
                case E3DLink.NorthEastBottom:
                    if (_y == 0)
                        return false;
                    if (_textureXSize == _x + 1)
                        return false;
                    if (_textureZSize == _z + 1)
                        return false;

                    coords.y = _y - 1;
                    coords.x = _x + 1;
                    coords.z = _z + 1;
                    return true;
                case E3DLink.EastBottom:
                    if (_y == 0)
                        return false;
                    if (_textureXSize == _x + 1)
                        return false;

                    coords.y = _y - 1;
                    coords.x = _x + 1;
                    return true;
                case E3DLink.SouthEastBottom:
                    if (_y == 0)
                        return false;
                    if (_textureXSize == _x + 1)
                        return false;
                    if (_z - 1 < 0)
                        return false;

                    coords.y = _y - 1;
                    coords.x = _x + 1;
                    coords.z = _z - 1;
                    return true;
                case E3DLink.SouthBottom:
                    if (_y == 0)
                        return false;
                    if (_z - 1 < 0)
                        return false;

                    coords.y = _y - 1;
                    coords.z = _z - 1;
                    return true;
                case E3DLink.SouthWestBottom:
                    if (_y == 0)
                        return false;
                    if (_x - 1 < 0)
                        return false;
                    if (_z - 1 < 0)
                        return false;

                    coords.y = _y - 1;
                    coords.x = _x - 1;
                    coords.z = _z - 1;
                    return true;
                case E3DLink.WestBottom:
                    if (_y == 0)
                        return false;
                    if (_x - 1 < 0)
                        return false;

                    coords.y = _y - 1;
                    coords.x = _x - 1;
                    return true;
                case E3DLink.NorthWestBottom:
                    if (_y == 0)
                        return false;
                    if (_x - 1 < 0)
                        return false;
                    if (_textureXSize == _z + 1)
                        return false;

                    coords.y = _y - 1;
                    coords.x = _x - 1;
                    coords.z = _z + 1;
                    return true;
                case E3DLink.North:
                    if (_textureZSize == _z + 1)
                        return false;

                    coords.z = _z + 1;
                    return true;
                case E3DLink.NorthEast:
                    if (_textureXSize == _x + 1)
                        return false;
                    if (_textureZSize == _z + 1)
                        return false;

                    coords.x = _x + 1;
                    coords.z = _z + 1;
                    return true;
                case E3DLink.East:
                    if (_textureXSize == _x + 1)
                        return false;

                    coords.x = _x + 1;
                    return true;
                case E3DLink.SouthEast:
                    if (_textureXSize == _x + 1)
                        return false;
                    if (_z - 1 < 0)
                        return false;

                    coords.x = _x + 1;
                    coords.z = _z - 1;
                    return true;
                case E3DLink.South:
                    if (_z - 1 < 0)
                        return false;

                    coords.z = _z - 1;
                    return true;
                case E3DLink.SouthWest:
                    if (_x - 1 < 0)
                        return false;
                    if (_z - 1 < 0)
                        return false;

                    coords.x = _x - 1;
                    coords.z = _z - 1;
                    return true;
                case E3DLink.West:
                    if (_x - 1 < 0)
                        return false;

                    coords.x = _x - 1;
                    return true;
                case E3DLink.NorthWest:
                    if (_x - 1 < 0)
                        return false;
                    if (_textureXSize == _z + 1)
                        return false;

                    coords.x = _x - 1;
                    coords.z = _z + 1;
                    return true;
                case E3DLink.Top:
                    if (_textureYSize == _y + 1)
                        return false;

                    coords.y = _y + 1;
                    return true;
                case E3DLink.NorthTop:
                    if (_textureYSize == _y + 1)
                        return false;
                    if (_textureZSize == _z + 1)
                        return false;

                    coords.y = _y + 1;
                    coords.z = _z + 1;
                    return true;
                case E3DLink.NorthEastTop:
                    if (_textureYSize == _y + 1)
                        return false;
                    if (_textureXSize == _x + 1)
                        return false;
                    if (_textureZSize == _z + 1)
                        return false;

                    coords.y = _y + 1;
                    coords.x = _x + 1;
                    coords.z = _z + 1;
                    return true;
                case E3DLink.EastTop:
                    if (_textureYSize == _y + 1)
                        return false;
                    if (_textureXSize == _x + 1)
                        return false;

                    coords.y = _y + 1;
                    coords.x = _x + 1;
                    return true;
                case E3DLink.SouthEastTop:
                    if (_textureYSize == _y + 1)
                        return false;
                    if (_textureXSize == _x + 1)
                        return false;
                    if (_z - 1 < 0)
                        return false;

                    coords.y = _y + 1;
                    coords.x = _x + 1;
                    coords.z = _z - 1;
                    return true;
                case E3DLink.SouthTop:
                    if (_textureYSize == _y + 1)
                        return false;
                    if (_z - 1 < 0)
                        return false;

                    coords.y = _y + 1;
                    coords.z = _z - 1;
                    return true;
                case E3DLink.SouthWestTop:
                    if (_textureYSize == _y + 1)
                        return false;
                    if (_x - 1 < 0)
                        return false;
                    if (_z - 1 < 0)
                        return false;

                    coords.y = _y + 1;
                    coords.x = _x - 1;
                    coords.z = _z - 1;
                    return true;
                case E3DLink.WestTop:
                    if (_textureYSize == _y + 1)
                        return false;
                    if (_x - 1 < 0)
                        return false;

                    coords.y = _y + 1;
                    coords.x = _x - 1;
                    return true;
                case E3DLink.NorthWestTop:
                    if (_textureYSize == _y + 1)
                        return false;
                    if (_x - 1 < 0)
                        return false;
                    if (_textureXSize == _z + 1)
                        return false;

                    coords.y = _y + 1;
                    coords.x = _x - 1;
                    coords.z = _z + 1;
                    return true;
                case E3DLink.None:
                    return false;
            }
            return false;
        }
        #endregion -----------------------------------------------------------------

        #region Is In Range -----------------------------------------------------------------
        public static bool IsInRange(this float _value, float _min, float _max) => (_value >= _min && _value <= _max);

        public static bool IsInRange(this int _value, int _min, int _max) => (_value >= _min && _value <= _max);

        public static bool IsInRange(this Vector2 _coords, int _xMinInclusive, int _xMaxInclusive, int _yMinInclusive, int _yMaxInclusive) =>
            _coords.x >= _xMinInclusive && _coords.x <= _xMaxInclusive && _coords.y >= _yMinInclusive && _coords.y <= _yMaxInclusive;
        #endregion -----------------------------------------------------------------

        /// <summary>
        /// Kleiner Gauss formula.
        /// </summary>
        public static int KleinerGauss(int _val) => (int)((_val * (_val + 1)) * 0.5f);

        /// <summary>
        /// by https://www.habrador.com/tutorials/math/5-line-line-intersection/
        /// </summary>
        //Line segment-line segment intersection in 2d space by using the dot product
        //p1 and p2 belongs to line 1, and p3 and p4 belongs to line 2 
        public static bool AreLineSegmentsIntersectingDotProduct(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
        {
            bool isIntersecting = false;

            if (IsPointsOnDifferentSides(p1, p2, p3, p4) && IsPointsOnDifferentSides(p3, p4, p1, p2))
            {
                isIntersecting = true;
            }

            return isIntersecting;
        }

        //Are the points on different sides of a line?
        private static bool IsPointsOnDifferentSides(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
        {
            bool isOnDifferentSides = false;

            //The direction of the line
            Vector3 lineDir = p2 - p1;

            //The normal to a line is just flipping x and z and making z negative
            Vector3 lineNormal = new Vector3(-lineDir.z, lineDir.y, lineDir.x);

            //Now we need to take the dot product between the normal and the points on the other line
            float dot1 = Vector3.Dot(lineNormal, p3 - p1);
            float dot2 = Vector3.Dot(lineNormal, p4 - p1);

            //If you multiply them and get a negative value then p3 and p4 are on different sides of the line
            if (dot1 * dot2 < 0f)
            {
                isOnDifferentSides = true;
            }

            return isOnDifferentSides;
        }

        public static float CosAngle(float _angle) => Mathf.Cos(((float)Mathf.PI / 180) * _angle);


        public static float SinAngle(float _angle) => Mathf.Sin(((float)Mathf.PI / 180) * _angle);

        /// <summary>
        /// Gets a file string without extensions.
        /// </summary>
        /// <param name="_file">The file</param>
        /// <returns>File string without extension</returns>
        public static string GetFileNameWithoutExtension(FileInfo _file)
        {
            string[] dotSplits = _file.Name.Split('.');
            string nameWithoutExtension = "";
            for (int i = 0; i < dotSplits.Length - 1; i++)
            {
                nameWithoutExtension += dotSplits[i] + ".";
            }
            return nameWithoutExtension.Remove(nameWithoutExtension.Length - 1);
        }

        /// <summary>
        /// This method is used because the a seperate collider is used for picking up the weapon and the child 
        /// can be easily rotated in several ways without interrupting the trajectory of the transform.
        /// 
        /// by https://github.com/Nova840/Miscellaneous/blob/master/Follow.cs
        /// </summary>
        public static void AlignParentWithChild(this Transform _parent, Transform _child, Vector3 _childStartLocalPosition, Quaternion _childStartLocalRotation)
        {
            _parent.position = _child.position;
            _child.RotateAround(_child.position, _child.forward, -_childStartLocalRotation.eulerAngles.z);
            _child.RotateAround(_child.position, _child.right, -_childStartLocalRotation.eulerAngles.x);
            _child.RotateAround(_child.position, _child.up, -_childStartLocalRotation.eulerAngles.y);
            _parent.rotation = _child.rotation;
            _parent.position += -_parent.right * _childStartLocalPosition.x;
            _parent.position += -_parent.up * _childStartLocalPosition.y;
            _parent.position += -_parent.forward * _childStartLocalPosition.z;
            _child.localRotation = _childStartLocalRotation;
            _child.localPosition = _childStartLocalPosition;
        }

        /// <summary>
        /// http://howlingmoonsoftware.com/wordpress/leading-a-target/ watch out when browsing this page. It is a little phishy ...
        /// </summary>
        /// <param name="_originPos"></param>
        /// <param name="_targetPosition"></param>
        /// <param name="_targetVelocity"></param>
        /// <param name="_shotVelocity"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public static Vector3 GetTargetPositionAfterTime2DNoAcceleration(Vector3 _originPos, Vector3 _targetPosition, Vector3 _targetVelocity, float _shotVelocity)
        {
            Vector3 direction = _targetPosition - _originPos;

            float a = Vector3.Dot(_targetVelocity, _targetVelocity) - _shotVelocity * _shotVelocity;
            float b = 2f * Vector3.Dot(_targetVelocity, direction);
            float c = Vector3.Dot(direction, direction);

            float det = b * b - 4f * a * c;

            // det < 0 -> no solutions
            if (det <= 0f)
                return _targetPosition;

            float t = 2f * c / (Mathf.Sqrt(det) - b);
            return t * _targetVelocity + _targetPosition;
        }

        public static Vector3 GetClosestPointOn2DAABB(Vector3 _point, Vector3 _halfExtents)
        {
            bool insideX = IsInRange(_point.x, -_halfExtents.x, _halfExtents.x);
            bool insideY = IsInRange(_point.z, -_halfExtents.z, _halfExtents.z);
            bool pointInsideRectangle = insideX && insideY;

            if (!pointInsideRectangle)
            {
                _point.x = Mathf.Clamp(_point.x, -_halfExtents.x, _halfExtents.x);
                _point.z = Mathf.Clamp(_point.z, -_halfExtents.z, _halfExtents.z);
                return _point;
            }

            Vector3 distanceToPositiveBounds = _halfExtents - _point;
            Vector3 distanceToNegativeBounds = -_halfExtents - _point;

            float smallestX = Mathf.Min(distanceToPositiveBounds.x, -distanceToNegativeBounds.x);
            float smallestZ = Mathf.Min(distanceToPositiveBounds.z, -distanceToNegativeBounds.z);
            float smallestDistance = Mathf.Min(smallestX, smallestZ);

            if (smallestDistance == distanceToPositiveBounds.x)
                _point.x = _halfExtents.x;
            else if (smallestDistance == -distanceToNegativeBounds.x)
                _point.x = -_halfExtents.x;
            else if (smallestDistance == distanceToPositiveBounds.z)
                _point.z = _halfExtents.z;
            else
                _point.z = -_halfExtents.z;

            return _point;
        }

        public static List<KeyValuePair<Vector3, Vector3>> GetGizmoAABBLines(Vector3 _center, Vector3 _halfExtents)
        {
            Vector3 xzMinBoundsLow = _center - _halfExtents;
            Vector3 xzMinBoundsHigh = xzMinBoundsLow + Vector3.up * _halfExtents.y * 2;

            Vector3 xzMaxBoundsHigh = _center + _halfExtents;
            Vector3 xzMaxBoundsLow = xzMaxBoundsHigh + Vector3.down * _halfExtents.y * 2;

            Vector3 xMinzMaxBoundsLow = new Vector3(xzMinBoundsLow.x, xzMinBoundsLow.y, xzMaxBoundsLow.z);
            Vector3 xMinzMaxBoundsHigh = xMinzMaxBoundsLow + Vector3.up * _halfExtents.y * 2;
            Vector3 zMinxMaxBoundsLow = new Vector3(xzMaxBoundsLow.x, xzMinBoundsLow.y, xzMinBoundsLow.z);
            Vector3 zMinxMaxBoundsHigh = zMinxMaxBoundsLow + Vector3.up * _halfExtents.y * 2;

            List<KeyValuePair<Vector3, Vector3>> gizmoLines = new List<KeyValuePair<Vector3, Vector3>>();
            // bottom left corner low
            gizmoLines.Add(new KeyValuePair<Vector3, Vector3>(xzMinBoundsLow, xzMinBoundsHigh));
            gizmoLines.Add(new KeyValuePair<Vector3, Vector3>(xzMinBoundsLow, xMinzMaxBoundsLow));
            gizmoLines.Add(new KeyValuePair<Vector3, Vector3>(xzMinBoundsLow, zMinxMaxBoundsLow));

            // bottom left corner high
            gizmoLines.Add(new KeyValuePair<Vector3, Vector3>(xzMinBoundsHigh, zMinxMaxBoundsHigh)); 
            gizmoLines.Add(new KeyValuePair<Vector3, Vector3>(xzMinBoundsHigh, xMinzMaxBoundsHigh));

            // top right corner low
            gizmoLines.Add(new KeyValuePair<Vector3, Vector3>(xzMaxBoundsLow, xzMaxBoundsHigh));
            gizmoLines.Add(new KeyValuePair<Vector3, Vector3>(xzMaxBoundsLow, xMinzMaxBoundsLow));
            gizmoLines.Add(new KeyValuePair<Vector3, Vector3>(xzMaxBoundsLow, zMinxMaxBoundsLow));
            
            // top right corner high
            gizmoLines.Add(new KeyValuePair<Vector3, Vector3>(xzMaxBoundsHigh, zMinxMaxBoundsHigh));
            gizmoLines.Add(new KeyValuePair<Vector3, Vector3>(xzMaxBoundsHigh, xMinzMaxBoundsHigh));
            
            // bottom right corner
            gizmoLines.Add(new KeyValuePair<Vector3, Vector3>(xMinzMaxBoundsLow, xMinzMaxBoundsHigh));

            // top left corner
            gizmoLines.Add(new KeyValuePair<Vector3, Vector3>(zMinxMaxBoundsLow, zMinxMaxBoundsHigh));
            return gizmoLines;
        }

        public static IEnumerable<T> Randomize<T>(this IEnumerable<T> _source)
        {
            return _source.OrderBy(x => RNG.Range(0, int.MaxValue));
        }
    }
}