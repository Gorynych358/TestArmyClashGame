using UnityEngine;
using VContainer;
using System.Collections.Generic;
using VContainer.Unity;

namespace ACT.Scripts
{
    public class UnitFactory : IUnitFactory
    {
        private readonly Dictionary<UnitTypes, UnitConfigSO> _configs;
        private readonly Dictionary<string, GameObject> _prefabs;
        private readonly IObjectResolver _resolver;

        public UnitFactory(
            Dictionary<UnitTypes, UnitConfigSO> configs,
            Dictionary<string, GameObject> prefabs,
            IObjectResolver resolver)
        {
            _configs = configs;
            _prefabs = prefabs;
            _resolver = resolver;
        }

        public Unit Create(
            UnitTypes type, 
            Transform parent = null, 
            Vector3 position = default)
        {
            var config = _configs[type];

            
            GameObject modelPrefab =
                config.Shape == ShapeModifierType.Cube ? _prefabs["CubePrefab"] : _prefabs["SpherePrefab"];

            GameObject unitPrefab = _prefabs["UnitPrefab"];
            var unitGO = _resolver.Instantiate(unitPrefab, position, Quaternion.identity, parent);
            var unit = unitGO.GetComponent<Unit>();

            Transform unitContainer = unit.ModelRoot;
            var model = _resolver.Instantiate(modelPrefab, position, Quaternion.identity, unitContainer);
            if(model == null)
            {
                Debug.LogError("UnitFactory Error: Game object ModelRoot not found!");
                return null;
            }

            SetupModel(model, config);
            
            unit.Initialize(config);

            return unit;
        }

        private void SetupModel(GameObject unitModel, UnitConfigSO config )
        {
            // Цвет
            var renderer = unitModel.GetComponentInChildren<Renderer>();
            renderer.material.color = config.ColorMod.ColorDef;

            // Размер
            float scale = config.SizeMod.SizeScaleFactor;
            unitModel.transform.localScale = Vector3.one * scale;

            //Позиция по вертикали:
            unitModel.transform.localPosition = new Vector3(0, scale*0.5f, 0);
        }
    }
}
