using UnityEngine;

namespace Framework.Core
{
    [CreateAssetMenu(fileName = "Currency Database", menuName = "Data/Currency/Database")]
    public class CurrencyDatabase : ScriptableObject
    {
        [SerializeField] Currency[] currencies;
        public Currency[] Currencies => currencies;
    }
}