using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Boot
{
    public class BootManager : MonoBehaviour
    {
        [Header("Boot UI")]
        [SerializeField] private GameObject bootUI;

        [Header("Boot Settings")]
        [SerializeField] private bool downloadPreloadAssets = true;

        private async Task RunBootFlowAync()
        {
            Debug.Log("[BootManager] 启动流程开始");

        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}