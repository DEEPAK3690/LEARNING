import React from "react";
import './license.css'
import { useState } from "react";

const LicenseView = () => {
    const DeviceInfo = {
        SerialNumber: "GRL-Device-001",
        PermanentLicense: ["GRL-Permanent-License-001", "GRL-Permanent-License-002", "GRL-Permanent-License-003", "GRL-Permanent-License-004", "GRL-Permanent-License-005"],
        TemporaryLicense: ["GRL-Temporary-License-001", "GRL-Temporary-License-002"],
    };


    const [ipAddress, setIpAddress] = useState('192.168.255.1');
    const [isConnected, setIsConnected] = useState(false);
    const [deviceInfo, setDeviceInfo] = useState(DeviceInfo);

    const [selectedPermanent, setSelectedPermanent] = useState("");
    const [selectedTemporary, setSelectedTemporary] = useState("");
    const [statusMessage, setStatusMessage] = useState("");

    const [PermanentLicenses, setPermanentLicenses] = useState([]);
    const [TemporaryLicenses, setTemporaryLicenses] = useState([]);
    const [removedPermanentLicenses, setRemovedPermanentLicenses] = useState([]);
    const [removedTemporaryLicenses, setRemovedTemporaryLicenses] = useState([]);

    const addlicense = (type) => {
        if (type === "permanent" && selectedPermanent) {
            if (PermanentLicenses.includes(selectedPermanent)) {
                setStatusMessage("Permanent license already added.");
                return;
            }

            setPermanentLicenses((prev) => {
                const updated = [...prev, selectedPermanent];
                console.log("Permanent Licenses:", updated);
                return updated;
            });
            setRemovedPermanentLicenses((prev) => prev.filter((license) => license !== selectedPermanent));
            setStatusMessage("Permanent license added.");
        } else if (type === "temporary" && selectedTemporary) {
            if (TemporaryLicenses.includes(selectedTemporary)) {
                setStatusMessage("Temporary license already added.");
                return;
            }

            setTemporaryLicenses((prev) => {
                const updated = [...prev, selectedTemporary];
                console.log("Temporary Licenses:", updated);
                return updated;
            });
            setRemovedTemporaryLicenses((prev) => prev.filter((license) => license !== selectedTemporary));
            setStatusMessage("Temporary license added.");
        }
    };

    const removelicense = (type) => {
        if (type === "permanent" && selectedPermanent) {
            if (PermanentLicenses.includes(selectedPermanent)) {
                setPermanentLicenses((prev) => {
                    const updated = prev.filter((license) => license !== selectedPermanent);
                    console.log("Permanent Licenses:", updated);
                    return updated;
                });
                setStatusMessage("Permanent license removed from added list.");
                return;
            }

            if (removedPermanentLicenses.includes(selectedPermanent)) {
                setStatusMessage("Permanent license already marked for removal.");
                return;
            }

            setRemovedPermanentLicenses((prev) => [...prev, selectedPermanent]);
            setStatusMessage("Permanent license marked for removal.");
        } else if (type === "temporary" && selectedTemporary) {
            if (TemporaryLicenses.includes(selectedTemporary)) {
                setTemporaryLicenses((prev) => {
                    const updated = prev.filter((license) => license !== selectedTemporary);
                    console.log("Temporary Licenses:", updated);
                    return updated;
                });
                setStatusMessage("Temporary license removed from added list.");
                return;
            }

            if (removedTemporaryLicenses.includes(selectedTemporary)) {
                setStatusMessage("Temporary license already marked for removal.");
                return;
            }

            setRemovedTemporaryLicenses((prev) => [...prev, selectedTemporary]);
            setStatusMessage("Temporary license marked for removal.");
        }
    };

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

                {isConnected && (
                    <div className="License-info">
                        <h2>License Information</h2>
                        <p>Serial Number: {deviceInfo.SerialNumber}</p>
                        <p><span>Permanent Licenses:</span> {deviceInfo.PermanentLicense.join(", ")}</p>
                        <p><span>Temporary Licenses:</span> {deviceInfo.TemporaryLicense.join(", ")}</p>
                    </div>)}
            </div>

            <div className="License-Update">
                <h2>License Update</h2>
                {isConnected ? (
                    <>
                        <div className="update-license">
                            <div className="permanent-license">
                                <label htmlFor="permanent-license">Add Permanent License: </label>
                                <select
                                    id="permanent-license"
                                    value={selectedPermanent}
                                    onChange={(e) => setSelectedPermanent(e.target.value)}
                                >
                                    <option value="">Select a permanent license</option>
                                    <option value="GRL-Permanent-License-006">GRL-Permanent-License-006</option>
                                    <option value="GRL-Permanent-License-007">GRL-Permanent-License-007</option>
                                    <option value="GRL-Permanent-License-008">GRL-Permanent-License-008</option>
                                    <option value="GRL-Permanent-License-009">GRL-Permanent-License-009</option>
                                    <option value="GRL-Permanent-License-010">GRL-Permanent-License-010</option>
                                </select>
                                <button onClick={() => addlicense("permanent")}>Add</button>
                                <button onClick={() => removelicense("permanent")}>Remove</button>
                            </div>
                            <div className="temporary-license">
                                <label htmlFor="temp-license">Add Temp License: </label>
                                <select
                                    id="temp-license"
                                    value={selectedTemporary}
                                    onChange={(e) => setSelectedTemporary(e.target.value)}
                                >
                                    <option value="">Select a temporary license</option>
                                    <option value="GRL-Temporary-License-003">GRL-Temporary-License-003</option>
                                    <option value="GRL-Temporary-License-004">GRL-Temporary-License-004</option>
                                    <option value="GRL-Temporary-License-005">GRL-Temporary-License-005</option>
                                </select>
                                <button onClick={() => addlicense("temporary")}>Add</button>
                                <button onClick={() => removelicense("temporary")}>Remove</button>
                            </div>
                        </div>

                        <div className="license-viewer">
                            <div className="viewer-card">
                                <h3>Added Permanent Licenses</h3>
                                <p><span>Permanent:</span> {PermanentLicenses.length ? PermanentLicenses.join(", ") : "None"}</p>
                            </div>
                            <div className="viewer-card">
                                <h3>Added Temporary Licenses</h3>
                                <p><span>Temporary:</span> {TemporaryLicenses.length ? TemporaryLicenses.join(", ") : "None"}</p>
                            </div>
                        </div>
                        {statusMessage && <p className="status-message">{statusMessage}</p>}

                        <div className="remove-license">
                            <button >Update</button>
                        </div>
                    </>
                ) : (
                    <p>Please connect to a device to update licenses.</p>
                )}
            </div>

        </div>
    );
}

export default LicenseView;