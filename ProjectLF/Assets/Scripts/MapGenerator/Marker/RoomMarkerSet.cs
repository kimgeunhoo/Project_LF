using UnityEngine;

namespace ModularBSP.Marker
{
    [System.Serializable]
    public class RoomMarkerSet : MonoBehaviour
    {
        [SerializeField]
        private GameObject StartMarkerPrefab;
        [SerializeField]
        private GameObject ShopMarkerPrefab;
        [SerializeField]
        private GameObject StairMarkerPrefab;
        [SerializeField]
        private GameObject EncounterMarkerPrefab;
        [SerializeField]
        private GameObject EnemyMarkerPrefab;

    }
}
