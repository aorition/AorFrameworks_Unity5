using System;
using System.Collections.Generic;
using UnityEngine;

namespace YoukiaUnity.CinemaSystem
{
    public class CinemaSubMountPoint : MonoBehaviour, ICinemaGizmoPoint
    {

        [SerializeField]
        private bool FollowSubMountPoint = false;

        [SerializeField]
        private string _subMountPointPath;

        [SerializeField]
        private Transform _mountRoot;

        private Transform _subMountPoint;

        public Transform SubMountPoint
        {
            get { return _subMountPoint;}
        }

        public void reset()
        {
            _subMountPoint = null;
        }

        public Transform getSubMountPoint(string subPath)
        {
            if (!_mountRoot) return null;
            if (string.IsNullOrEmpty(subPath)) return _mountRoot;

            string[] paths = subPath.Split('/');

            if (paths != null && paths.Length > 0)
            {

                Transform r = _mountRoot;
                Transform s;
                int i, len = paths.Length;
                for (i = 0; i < len; i++)
                {
                    s = r.Find(paths[i]);
                    if (s)
                    {
                        r = s;
                    }
                    else
                    {
                        return null;
                    }
                }

                return r;
            }

            return _mountRoot;
        }

        private int _findNum = 0;
        private int _findLimit = 5;

        public bool isInit
        {
            get { return (_subMountPoint != null); }
        }

        public void init()
        {
            if (!string.IsNullOrEmpty(_subMountPointPath) && _mountRoot)
            {
                _subMountPoint = getSubMountPoint(_subMountPointPath);
                _findNum ++;
            } 
        }

        private void Update()
        {

            if (!_subMountPoint && _findNum < _findLimit)
            {
                init();
            }

            if (!enabled || !_subMountPoint) return;

            if (FollowSubMountPoint)
            {
                transform.rotation =  _subMountPoint.rotation;
                transform.position = _subMountPoint.position;
            }
            else
            {
                _subMountPoint.rotation = transform.rotation;
                _subMountPoint.position = transform.position;
            }
        }


    }
}
