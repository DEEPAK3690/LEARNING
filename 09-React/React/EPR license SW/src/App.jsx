
import React from 'react'
import './App.css'
import Header from './components/Header'
import Login from './components/Login'
import LicenseView from './components/Licenseview'
import { useState } from 'react'


function App() {
    const [isLoggedIn, setIsLoggedIn] = useState(false);

  return (
    <>
      <Header isLoggedIn={isLoggedIn} setIsLoggedIn={setIsLoggedIn} />
      {isLoggedIn ? <LicenseView /> : <Login isLoggedIn={isLoggedIn} setIsLoggedIn={setIsLoggedIn} />}
    </>
  )
}

export default App
