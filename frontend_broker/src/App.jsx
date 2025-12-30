import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';

import Navbar from './components/Navbar';
import Home from './pages/Home';
import AuthPage from './pages/AuthPage';
import MarketDashboard from './pages/Market';
import PortfolioPage from './pages/Portfolio';
import TransactionHistory from './pages/TransactionHistory';
import PositionsPage from './pages/PositionsPage';
import PositionDetails from './pages/PositionDetails';
import WalletHistory from './pages/WalletHistory';
import AdminDashboardPage from './pages/AdminDashboardPage';
import AdminUsersPage from './Pages/AdminUsers';
import AdminReports from './Pages/AdminReports';

const AccessDenied = () => (
    <div className="d-flex flex-column justify-content-center align-items-center h-100 text-center">
        <h1 className="display-4 text-danger">Brak dostępu</h1>
        <p>Nie masz uprawnień do przeglądania tej strony.</p>
    </div>
);

const NotFound = () => (
    <div className="d-flex flex-column justify-content-center align-items-center h-100 text-center">
        <h1 className="display-1 fw-bold text-secondary">404</h1>
        <p className="fs-4 mt-3">Strona nie została znaleziona.</p>
        <a href="/" className="btn btn-primary mt-3">Wróć na stronę główną</a>
    </div>
);

const App = () => {
    const userRole = localStorage.getItem('userRole');

    return (
        <BrowserRouter>
            <div className="d-flex flex-column min-vh-100 bg-light">

                <Navbar />

                <main className="flex-grow-1 d-flex flex-column">
                    <div className="container d-flex flex-column flex-grow-1 py-4">
                        <Routes>

                         
                            <Route
                                path="/"
                                element={
                                    userRole === 'Admin' ? <Navigate to="/admin" replace /> : <Home />
                                }
                            />

                            <Route
                                path="/login"
                                element={
                                    userRole === 'User' ? <Navigate to="/market" /> :
                                        userRole === 'Admin' ? <Navigate to="/admin" /> :
                                            <AuthPage />
                                }
                            />

                            {userRole === "User" && (
                                <>
                                    <Route path="/market" element={<MarketDashboard />} />
                                    <Route path="/portfolio" element={<PortfolioPage />} />
                                    <Route path="/positions" element={<PositionsPage />} />
                                    <Route path="/positions/:ticker" element={<PositionDetails />} />
                                    <Route path="/transactionhistory" element={<TransactionHistory />} />
                                    <Route path="/wallethistory" element={<WalletHistory />} />
                                </>
                            )}

                           
                            <Route
                                path="/admin"
                                element={userRole === "Admin" ? <AdminDashboardPage /> : <AccessDenied />}
                            />
                            <Route
                                path="/admin/users"
                                element={userRole === "Admin" ? <AdminUsersPage /> : <AccessDenied />}
                            />
                      
                            <Route
                                path="/admin/reports"
                                element={userRole === "Admin" ? <AdminReports /> : <AccessDenied />}
                            />

                            
                            <Route path="/404" element={<NotFound />} />
                            <Route path="*" element={<Navigate to="/404" replace />} />
                        </Routes>
                    </div>
                </main>
            </div>
        </BrowserRouter>
    );
};

export default App;