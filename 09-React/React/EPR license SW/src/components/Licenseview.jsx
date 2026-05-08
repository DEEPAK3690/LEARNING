import { useState } from "react";
import './license.css'
import axios from "axios";

const BASE_URL = import.meta.env.VITE_API_BASE_URL;

const LicenseView = () => {

    const [ipAddress, setIpAddress] = useState('192.168.255.1');
    const [isConnected, setIsConnected] = useState(false);
    const [deviceInfo, setDeviceInfo] = useState({});

    const [selectedPermanent, setSelectedPermanent] = useState("");
    const [selectedTemporary, setSelectedTemporary] = useState("");
    const [statusMessage, setStatusMessage] = useState("");

    const [PermanentLicenses, setPermanentLicenses] = useState([]);
    const [TemporaryLicenses, setTemporaryLicenses] = useState([]);
    const [availablePermanentOptions, setAvailablePermanentOptions] = useState([]);
    const [availableTemporaryOptions, setAvailableTemporaryOptions] = useState([]);

    const [removedPermanentLicenses, setRemovedPermanentLicenses] = useState([]);
    const [removedTemporaryLicenses, setRemovedTemporaryLicenses] = useState([]);

    const [TempLicenseDate, setUpdateDate] = useState({});

    const handleDateChange = (e) => {
        const { name, value } = e.target;
        setUpdateDate((prev) => ({ ...prev, [name]: value }));
        console.log("Temp License Date:", { ...TempLicenseDate, [name]: value });
    }

    const updateLicense = async () => {
        try {
            const { from, to } = TempLicenseDate;
            const response = await axios.post(
                `${BASE_URL}/api/connection/updateLicense`,
                { PermanentLicenses, TemporaryLicenses, validFrom: from, validTo: to }
            );
            console.log('Update License success:', response.data);
            setStatusMessage("License updated successfully.");
        } catch (error) {
            console.error('Update License error:', error);
            setStatusMessage("Failed to update license. Please try again.");
        }
    };

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

    const handleConnect = async () => {
        try {
            const connectResponse = await axios.post(
                `${BASE_URL}/api/connection/connect`,
                { ipAddress }  // Automatic JSON conversion
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

                console.log('Calling licenseList API...');

                const licenseResponse = await axios.get(
                    `${BASE_URL}/api/connection/licenseList`
                );
                const licenseData = licenseResponse.data;

                console.log('License List API success:', licenseData);

                const permanentFromApi = Array.isArray(licenseData?.permanentlicense)
                    ? licenseData.permanentlicense
                    : [];

                const temporaryFromApi = Array.isArray(licenseData?.templicense)
                    ? licenseData.templicense
                    : [];

                setAvailablePermanentOptions(permanentFromApi);
                setAvailableTemporaryOptions(temporaryFromApi);
            }
        } catch (error) {
            console.error('Error:', error);  // Automatic error handling
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
                <button onClick={handleConnect}>Connect</button>
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
                                    {availableTemporaryOptions.map((license) => (
                                        <option key={license} value={license}>{license}</option>
                                    ))}
                                </select>
                                <button onClick={() => addlicense("temporary")}>Add</button>
                                <button onClick={() => removelicense("temporary")}>Remove</button>
                                <div>
                                    <fieldset>
                                        <legend>DATE: </legend>
                                        <label htmlFor="date-from">FROM: </label>
                                        <input type="date" id="date-from" name="from" onChange={handleDateChange} />
                                        <label htmlFor="date-to">TO: </label>
                                        <input type="date" id="date-to" name="to" onChange={handleDateChange} />
                                    </fieldset>
                                </div>
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

                        <div className="update-button">
                            <button onClick={updateLicense}>Update License</button>
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