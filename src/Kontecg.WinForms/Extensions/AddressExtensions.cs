using System;
using System.Collections.Generic;
using DevExpress.XtraMap;
using Kontecg.HumanResources;

namespace Kontecg.Extensions
{
    public static class AddressExtensions
    {
        public static GeoPoint ToGeoPoint(this Address address)
        {
            return (address != null) ? new GeoPoint(address.Latitude, address.Longitude) : null;
        }

        public static void ZoomTo(this DevExpress.Map.Dashboard.IZoomToRegionService zoomService,
            IEnumerable<Address> addresses, double margin = 0.25)
        {
            GeoPoint ptA = null;
            GeoPoint ptB = null;
            foreach (var address in addresses)
            {
                if (ptA == null)
                {
                    ptA = address.ToGeoPoint();
                    ptB = address.ToGeoPoint();
                    continue;
                }

                GeoPoint pt = address.ToGeoPoint();
                if (pt == null || object.Equals(pt, new GeoPoint(0, 0)))
                    continue;
                ptA.Latitude = Math.Min(ptA.Latitude, pt.Latitude);
                ptA.Longitude = Math.Min(ptA.Longitude, pt.Longitude);
                ptB.Latitude = Math.Max(ptB.Latitude, pt.Latitude);
                ptB.Longitude = Math.Max(ptB.Longitude, pt.Longitude);
            }

            ZoomCore(zoomService, ptA, ptB, margin);
        }

        public static void ZoomTo(this DevExpress.Map.Dashboard.IZoomToRegionService zoomService, Address pointA, Address pointB, double margin = 0.2)
        {
            ZoomCore(zoomService, pointA.ToGeoPoint(), pointB.ToGeoPoint(), margin);
        }

        private static void ZoomCore(DevExpress.Map.Dashboard.IZoomToRegionService zoomService, GeoPoint ptA, GeoPoint ptB, double margin)
        {
            if (ptA == null || ptB == null || zoomService == null) return;
            double latPadding = CalculatePadding(ptB.Latitude - ptA.Latitude, margin);
            double longPadding = CalculatePadding(ptB.Longitude - ptA.Longitude, margin);
            zoomService.ZoomToRegion(
                new GeoPoint(ptA.Latitude - latPadding, ptA.Longitude - longPadding),
                new GeoPoint(ptB.Latitude + latPadding, ptB.Longitude + longPadding),
                new GeoPoint(0.5 * (ptA.Latitude + ptB.Latitude), 0.5 * (ptA.Longitude + ptB.Longitude)));
        }

        private static double CalculatePadding(double delta, double margin)
        {
            if (delta > 0)
                return Math.Max(0.1, delta * margin);
            if (delta < 0)
                return Math.Min(-0.1, delta * margin);
            return 0;
        }
    }
}