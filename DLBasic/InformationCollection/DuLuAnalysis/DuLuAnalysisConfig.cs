using System.Collections;
using System.Collections.Generic;
using InformationCollection;
using UnityEngine;

namespace IC.IcDuLu
{
    [CreateAssetMenu(menuName = "InformationGathering/Create DuLuAnalysisConfig")]
    public class DuLuAnalysisConfig : ICConfigBase
    {
        public override string IcMgrPath => "IC.IcDuLu.DuLuAnalysisMgr";

        [Header("渠道id")]
        public int TypeID;
    }
}

