import './Header.css'


const Header = ({isLoggedIn, setIsLoggedIn}) => {
    const handleLogout = () => {
        setIsLoggedIn(false);
    };

    return (
        <div>
            <header className="header" >
                <h1>
                    GRL License Manager
                </h1>
                {isLoggedIn && (
                    <div className="log-out">
                        <button onClick={handleLogout}>Logout</button>
                    </div>
                )}
            </header>
        </div>
    );
};

export default Header;