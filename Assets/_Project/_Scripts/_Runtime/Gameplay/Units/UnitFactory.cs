using UnityEngine;
using VContainer;
using System.Collections.Generic;
using VContainer.Unity;

namespace ACT.Runtime.Gameplay.Units
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

            SetupModel(model.transform, config);
            
            unit.BindConfig(config);

            return unit;
        }

        private void SetupModel(Transform modelTransform, UnitConfigSO config )
        {
            // Цвет
            //Для отображения цвета модели из модификатора добавил специальный GameObject
            //с именем ModifierColor. Служит для отображения типа юнита.
            if(modelTransform.childCount > 1 && 
                modelTransform.GetChild(0).name == "ModifierColor")//Проверяем есть ли этот GameObject
            {
                var renderer = modelTransform.transform.GetChild(0).gameObject.GetComponentInChildren<Renderer>();
                renderer.material.color = config.ColorMod.ColorDef;
            }
            // Размер
            float scale = config.SizeMod.SizeScaleFactor;
            modelTransform.localScale = Vector3.one * scale;
            
            //Позиция по вертикали:
            modelTransform.localPosition = new Vector3(0, scale*0.5f, 0);
        }
    }
}
