import React from "react";
import './license.css'
import { useState } from "react";

const LicenseView = () => {
    const DeviceInfo = {
        SerialNumber: "GRL-Device-001",
        PermanentLicense: ["GRL-Permanent-License-001", "GRL-Permanent-License-002", "GRL-Permanent-License-003"],
        TemporaryLicense: ["GRL-Temporary-License-001", "GRL-Temporary-License-002"],
    };


    const [ipAddress, setIpAddress] = useState('192.168.255.1');
    const [isConnected, setIsConnected] = useState(false);
    const [deviceInfo, setDeviceInfo] = useState(DeviceInfo);

    const handleConnect = () => {
        // Handle connect logic here
        setIsConnected(true);
    };

    return (
        <div className="License-view-main">
            <div className="Connection-view">
                <label htmlFor="ip-address">IP Address:</label>
                <input
                    type="text"
                    id="ip-address"
                    placeholder="Enter IP Address"
                    value={ipAddress}
                    onChange={(e) => setIpAddress(e.target.value)}
                />
                <button onClick={handleConnect}>Connect</button>
                {isConnected && <p>Connected to {ipAddress}</p>}
            </div>
            {isConnected && (
                <div className="License-info">
                    <h2>License Information</h2>
                    <p>Serial Number: {deviceInfo.SerialNumber}</p>
                    <p><span>Permanent Licenses:</span> {deviceInfo.PermanentLicense.join(", ")}</p>
                    <p><span>Temporary Licenses:</span> {deviceInfo.TemporaryLicense.join(", ")}</p>
                </div>)}

        </div>
    );
}

export default LicenseView;