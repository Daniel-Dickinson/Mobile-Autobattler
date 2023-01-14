using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBears.Pooling
{
    public class PoolManager : MonoBehaviour
    {
        public static Dictionary<string, ItemPool> pools;
        private static PoolManager manager;

        //Mono
        private void Awake()
        {
            //Set as manager
            manager = this;
        }
        private void Update()
        {
            //Update item pools
            if (pools == null) return;
            foreach (KeyValuePair<string, ItemPool> pair in pools) pair.Value.UpdateItems(Time.deltaTime);
        }

        //Request
        public static Poolable RequestPoolable(Poolable source, Transform parent = null)
        {
            return RequestPoolable(source, Vector3.zero, Quaternion.identity, parent);
        }
        public static Poolable RequestPoolable(Poolable source, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            //Initialize pools
            if (pools == null) pools = new Dictionary<string, ItemPool>();

            //Source must be valid
            if (source == null) Debug.Log("Invalid poolable being requested");

            //Grab pool
            if (pools.TryGetValue(source.PoolID, out ItemPool pool))
            {
                //Grab item
                return pool.Request(manager, position, rotation, parent);
            }
            else
            {
                //Create pool
                ItemPool itemPool = new ItemPool(source);
                pools.Add(source.PoolID, itemPool);

                //Grab item
                return itemPool.Request(manager, position, rotation, parent);
            }
        }

        //Return
        public static bool ReturnPoolable(Poolable item)
        {
            if (item == null) Debug.Log("Invalid item being returned to pool");

            //Grab pool
            if (pools.TryGetValue(item.PoolID, out ItemPool pool))
            {
                pool.Return(manager, item);
                return true;
            }
            else
            {
                Debug.LogWarning("Poolable Instantiated directly! Please ");
                return false;
            }
        }

        //Destroyed
        public static void PoolableDestroyed(Poolable item)
        {
            //If item is pooled remove it
            if (pools.TryGetValue(item.PoolID, out ItemPool pool)) pool.Destroyed(item);
        }
    }

    public class ItemPool
    {
        public string poolID;
        public Poolable source;

        //Items
        public List<Poolable> pooledItems;
        public List<Poolable> activeItems;

        //Constructor
        public ItemPool(Poolable source)
        {
            this.poolID = source.PoolID;
            this.source = source;

            pooledItems = new List<Poolable>();
            activeItems = new List<Poolable>();
        }

        //Request
        public Poolable Request(PoolManager manager, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (pooledItems.Count > 0)
            {
                //Request an item
                Poolable request = pooledItems[0];
                pooledItems.RemoveAt(0);

                //Set parent
                if (parent != null) request.transform.SetParent(parent, false);

                //Position the item
                request.transform.position = position;
                request.transform.rotation = rotation;

                //Enable the item
                request.gameObject.SetActive(true);

                //Initialize the item
                request.PoolableInit();

                //Add to active
                activeItems.Add(request);

                return request;
            }
            else
            {
                //Account for null parent
                if (parent == null) parent = manager.transform;

                //Create a new item
                Poolable newItem = GameObject.Instantiate(source, position, rotation, parent);

                //Initialize the item
                newItem.PoolableInit();

                //Add to active
                activeItems.Add(newItem);

                return newItem;
            }
        }

        //Return
        public void Return(PoolManager manager, Poolable poolable)
        {
            //Remove from active
            activeItems.Remove(poolable);

            //Cleanup item
            poolable.PoolableReset();

            //Deactivate item
            poolable.gameObject.SetActive(false);

            //Parent to pool
            poolable.transform.SetParent(manager.transform);

            //Add to pool
            pooledItems.Add(poolable);
        }

        //Destoyed
        public void Destroyed(Poolable poolable)
        {
            //Clear item from active & pool
            pooledItems.Remove(poolable);
            activeItems.Remove(poolable);
        }

        //Update
        public void UpdateItems(float deltaTime)
        {
            for (int i = 0; i < activeItems.Count; i++)
            {
                activeItems[i].PoolableUpdate(deltaTime);
            }
        }
    }
}