using UnityEngine;

namespace ModularBSP.Config
{
    [System.Serializable]
    public class PathPrefabSet
    {
        [Header("Empty")]
        public GameObject Empty;

        [Header("Dead End")]
        public GameObject UpEnd;
        public GameObject RightEnd;
        public GameObject DownEnd;
        public GameObject LeftEnd;

        [Header("Straight")]
        public GameObject Horizontal;
        public GameObject Vertical;

        [Header("Corner")]
        public GameObject UpRightCorner;
        public GameObject RightDownCorner;
        public GameObject DownLeftCorner;
        public GameObject LeftUpCorner;

        [Header("T Junction")]
        public GameObject UpRightDownTJunction;
        public GameObject RightDownLeftTJunction;
        public GameObject DownLeftUpTJunction;
        public GameObject LeftUpRightTJunction;

        [Header("Cross")]
        public GameObject cross;
    }
}
