import { useState } from "react";
import './login.css'

const Login = ({ setIsLoggedIn }) => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');

    const handleSubmit = (event) => {
        event.preventDefault();
        const normalizedUsername = username.trim();
        const normalizedPassword = password.trim();

        if (normalizedUsername === 'GRL' && normalizedPassword === 'GRLUSER') {
            setIsLoggedIn(true);
            setError('');
        } else {
            setIsLoggedIn(false);
            setError('Invalid username or password.');
        }
    };

    return (
        <>
            <div className="login-main" >
                <h1>
                    License Manager Login
                </h1>
                <form className="login-form" onSubmit={handleSubmit}>
                    <label htmlFor="username">ID:</label>
                    <input
                        type="text"
                        id="username"
                        name="username"
                        value={username}
                        onChange={(e) => setUsername(e.target.value)}
                        placeholder="username"
                    />
                    <label htmlFor="password">Password:</label>
                    <input
                        type="password"
                        id="password"
                        name="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        placeholder="Password"
                    />
                    {error && <p className="login-error">{error}</p>}
                    <button type="submit" disabled={!username.trim() || !password.trim()}>Login</button>
                </form>
            </div>
        </>
    );
};

export default Login;