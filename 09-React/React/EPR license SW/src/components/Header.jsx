import './Header.css'


const Header = ({ isLoggedIn, setIsLoggedIn }) => {
    const handleLogout = () => {
        setIsLoggedIn(false);
    };

    return (
        <header className="header" >
            <h1>
                GRL License Manager
            </h1>
            <p className="header-subtitle">Manage connectivity and license lifecycle in one place</p>
            {isLoggedIn && (
                <div className="log-out">
                    <button onClick={handleLogout} aria-label="Logout from application">Logout</button>
                </div>
            )}
        </header>
    );
};

export default Header;