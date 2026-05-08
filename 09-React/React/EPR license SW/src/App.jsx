
import './App.css'
import Header from './components/Header'
import Login from './components/Login'
import LicenseView from './components/Licenseview'
import { useState } from 'react'


function App() {
  const [isLoggedIn, setIsLoggedIn] = useState(false)

  return (
    <div className="app-shell">
      <Header isLoggedIn={isLoggedIn} setIsLoggedIn={setIsLoggedIn} />
      <main className="app-main">
        {isLoggedIn ? <LicenseView /> : <Login setIsLoggedIn={setIsLoggedIn} />}
      </main>
    </div>
  )
}

export default App
