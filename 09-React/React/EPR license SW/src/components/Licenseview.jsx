import { useState,useEffect } from "react";
import './license.css'
import axios from "axios";

const BASE_URL = import.meta.env.VITE_API_BASE_URL;
const defaultDeviceInfo = {
    SerialNumber: "",
    PermanentLicense: [],
    TemporaryLicense: [],
};

const ipAddressPattern = /^(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}$/;

const LicenseView = () => {

    const [ipAddress, setIpAddress] = useState('192.168.255.1');
    const [isConnected, setIsConnected] = useState(false);
    const [deviceInfo, setDeviceInfo] = useState(defaultDeviceInfo);
    const [isConnecting, setIsConnecting] = useState(false);
    const [isUpdating, setIsUpdating] = useState(false);

    const [selectedPermanent, setSelectedPermanent] = useState("");
    const [selectedTemporary, setSelectedTemporary] = useState("");
    const [statusMessage, setStatusMessage] = useState("");

    const [permanentLicenses, setPermanentLicenses] = useState([]);
    const [temporaryLicenses, setTemporaryLicenses] = useState([]);
    const [availablePermanentOptions, setAvailablePermanentOptions] = useState([]);
    const [availableTemporaryOptions, setAvailableTemporaryOptions] = useState([]);

    const [removedPermanentLicenses, setRemovedPermanentLicenses] = useState([]);
    const [removedTemporaryLicenses, setRemovedTemporaryLicenses] = useState([]);

    const [tempLicenseDate, setUpdateDate] = useState({ from: "", to: "" });

    const handleDateChange = (e) => {
        const { name, value } = e.target;
        setUpdateDate((prev) => ({ ...prev, [name]: value }));
    }

    const updateLicense = async () => {
        if (!isConnected) {
            setStatusMessage("Connect to a device before updating licenses.");
            return;
        }

        if (tempLicenseDate.from && tempLicenseDate.to && tempLicenseDate.from > tempLicenseDate.to) {
            setStatusMessage("From date cannot be greater than To date.");
            return;
        }

        try {
            setIsUpdating(true);
            const { from, to } = tempLicenseDate;
            const response = await axios.post(
                `${BASE_URL}/api/connection/updateLicense`,
                { permanentLicenses, temporaryLicenses, validFrom: from, validTo: to }
            );
            console.log('Update License success:', response.data);
            setStatusMessage("License updated successfully.");
        } catch (error) {
            console.error('Update License error:', error);
            setStatusMessage("Failed to update license. Please try again.");
        } finally {
            setIsUpdating(false);
        }
    };

    const addLicense = (type) => {
        if (type === "permanent" && selectedPermanent) {
            if (permanentLicenses.includes(selectedPermanent)) {
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
            if (temporaryLicenses.includes(selectedTemporary)) {
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

    const removeLicense = (type) => {
        if (type === "permanent" && selectedPermanent) {
            if (permanentLicenses.includes(selectedPermanent)) {
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
            if (temporaryLicenses.includes(selectedTemporary)) {
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

    const handleConnect = async () => {
        if (!ipAddressPattern.test(ipAddress.trim())) {
            setStatusMessage("Enter a valid IPv4 address.");
            return;
        }

        try {
            setIsConnecting(true);
            setStatusMessage("");
            const connectResponse = await axios.post(
                `${BASE_URL}/api/connection/connect`,
                { ipAddress: ipAddress.trim() }
            );
            const connectData = connectResponse.data;

            if (connectData?.isConnected) {
                setIsConnected(true);
                setDeviceInfo({
                    SerialNumber: connectData.serialNumber || "",
                    PermanentLicense: Array.isArray(connectData.existingLicense) ? connectData.existingLicense : [],
                    TemporaryLicense: Array.isArray(connectData.existingTempLicense) ? connectData.existingTempLicense : [],
                });
                console.log('Connect API success:', connectData);
            } else {
                setIsConnected(false);
                setStatusMessage("Unable to connect to target device.");
            }
        } catch (error) {
            setIsConnected(false);
            setDeviceInfo(defaultDeviceInfo);
            setAvailablePermanentOptions([]);
            setAvailableTemporaryOptions([]);
            console.error('Error:', error);  // Automatic error handling
            setStatusMessage("Connection failed. Check API/server and try again.");
        } finally {
            setIsConnecting(false);
        }
    };

    // Watch isConnected state
    useEffect(() => {
        if (isConnected) {
            // Fetch license list when connected
            fetchLicenseList();
        }
    }, [isConnected]);  // Re-run when isConnected changes

    const fetchLicenseList = async () => {
        try {
            const licenseResponse = await axios.get(`${BASE_URL}/api/connection/licenseList`);
            const licenseData = licenseResponse.data;

            const permanentFromApi = Array.isArray(licenseData?.permanentlicense)
                ? licenseData.permanentlicense
                : [];
            const temporaryFromApi = Array.isArray(licenseData?.templicense)
                ? licenseData.templicense
                : [];

            setAvailablePermanentOptions(permanentFromApi);
            setAvailableTemporaryOptions(temporaryFromApi);
        } catch (error) {
            console.error('License List error:', error);
            setStatusMessage("Failed to load license options.");
        }
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
                <button onClick={handleConnect} disabled={isConnecting}>
                    {isConnecting ? "Connecting..." : "Connect"}
                </button>
                {isConnected ? <p>Connected to {ipAddress}</p> : <p>Not connected</p>}

                {deviceInfo && (
                    <div className="License-info">
                        <h2>License Information</h2>
                        <p>Serial Number: {deviceInfo?.SerialNumber || "N/A"}</p>
                        <p><span>Permanent Licenses:</span> {(deviceInfo?.PermanentLicense ?? []).join(", ") || "None"}</p>
                        <p><span>Temporary Licenses:</span> {(deviceInfo?.TemporaryLicense ?? []).join(", ") || "None"}</p>
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
                                    {availablePermanentOptions.map((license) => (
                                        <option key={license} value={license}>{license}</option>
                                    ))}
                                </select>
                                <button onClick={() => addLicense("permanent")} disabled={!selectedPermanent}>Add</button>
                                <button onClick={() => removeLicense("permanent")} disabled={!selectedPermanent}>Remove</button>
                            </div>
                            <div className="temporary-license">
                                <label htmlFor="temp-license">Add Temp License: </label>
                                <select
                                    id="temp-license"
                                    value={selectedTemporary}
                                    onChange={(e) => setSelectedTemporary(e.target.value)}
                                >
                                    <option value="">Select a temporary license</option>
                                    {availableTemporaryOptions.map((license) => (
                                        <option key={license} value={license}>{license}</option>
                                    ))}
                                </select>
                                <button onClick={() => addLicense("temporary")} disabled={!selectedTemporary}>Add</button>
                                <button onClick={() => removeLicense("temporary")} disabled={!selectedTemporary}>Remove</button>
                                <div>
                                    <fieldset>
                                        <legend>DATE: </legend>
                                        <label htmlFor="date-from">FROM: </label>
                                        <input type="date" id="date-from" name="from" value={tempLicenseDate.from} onChange={handleDateChange} />
                                        <label htmlFor="date-to">TO: </label>
                                        <input type="date" id="date-to" name="to" value={tempLicenseDate.to} onChange={handleDateChange} />
                                    </fieldset>
                                </div>
                            </div>
                        </div>

                        <div className="license-viewer">
                            <div className="viewer-card">
                                <h3>Added Permanent Licenses</h3>
                                <p><span>Permanent:</span> {permanentLicenses.length ? permanentLicenses.join(", ") : "None"}</p>
                            </div>
                            <div className="viewer-card">
                                <h3>Added Temporary Licenses</h3>
                                <p><span>Temporary:</span> {temporaryLicenses.length ? temporaryLicenses.join(", ") : "None"}</p>
                            </div>
                        </div>
                        {statusMessage && <p className="status-message">{statusMessage}</p>}

                        <div className="update-button">
                            <button
                                onClick={updateLicense}
                                disabled={isUpdating || (!permanentLicenses.length && !temporaryLicenses.length)}
                            >
                                {isUpdating ? "Updating..." : "Update License"}
                            </button>
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