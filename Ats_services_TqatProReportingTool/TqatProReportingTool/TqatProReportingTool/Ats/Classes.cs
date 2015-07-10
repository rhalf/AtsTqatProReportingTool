using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ats {
    enum ReportType {
        HISTORICAL,
        RUNNING,
        IDLING,
        GEOFENCE,
        ACC,
        EXTERNAL_POWER_CUT,
        OVERSPEED,
        TRACKERS,
        TRACKERS_GEOFENCE,
        TRACKERS_HISTORICAL,
        //URGENT,
        ALL_COMPANIES,
        ALL_TRACKERS
    }
    public enum ExportFileType {
        TXT,
        CSV,
        PDF
    }
    class Classes {
    }
}
