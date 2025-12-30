import React from 'react';
import { Link, useLocation } from 'react-router-dom';

const Navbar = () => {
    const location = useLocation();


    const token = localStorage.getItem('token');
    const userRole = localStorage.getItem('userRole');

    const handleLogout = () => {

        localStorage.removeItem('token');
        localStorage.removeItem('userEmail');
        localStorage.removeItem('userRole');

       
        window.location.href = '/';
    };

    const getLinkClass = (path) => {
        const isActive = location.pathname === path;
        return isActive ? "nav-link active fw-bold text-primary" : "nav-link text-secondary";
    };

    return (
        <nav className="navbar navbar-expand-lg navbar-light bg-white shadow-sm fixed-top" style={{ height: '70px' }}>
            <div className="container">
                <Link to="/" className="navbar-brand fw-bold text-primary fs-4">
                    BrokerApp
                </Link>

                <button className="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
                    <span className="navbar-toggler-icon"></span>
                </button>

                <div className="collapse navbar-collapse" id="navbarNav">
                    <ul className="navbar-nav me-auto mb-2 mb-lg-0">
                        <li className="nav-item">
                            <Link to="/" className={getLinkClass('/')}>Home</Link>
                        </li>

                        {userRole === 'User' && (
                            <>
                                <li className="nav-item"><Link to="/market" className={getLinkClass('/market')}>Rynek</Link></li>
                                <li className="nav-item"><Link to="/portfolio" className={getLinkClass('/portfolio')}>Portfel</Link></li>
                                <li className="nav-item"><Link to="/positions" className={getLinkClass('/positions')}>Moje Akcje</Link></li>

                                <li className="nav-item"><Link to="/wallethistory" className={getLinkClass('/wallethistory')}>Historia Portfela</Link></li>

                                <li className="nav-item"><Link to="/transactionhistory" className={getLinkClass('/transactionhistory')}>Zamknięte</Link></li>
                            </>
                        )}

                        {userRole === 'Admin' && (
                            <>
                                <li className="nav-item">
                                    <Link to="/admin" className={getLinkClass('/admin')}>
                                        <span className="text-danger fw-bold">Admin Panel</span>
                                    </Link>
                                </li>
                                <li className="nav-item"><Link to="/admin/users" className={getLinkClass('/admin/users')}>Użytkownicy</Link></li>

                                <li className="nav-item"><Link to="/admin/reports" className={getLinkClass('/admin/reports')}>Raporty SQL</Link></li>
                            </>
                        )}
                    </ul>

                    <div className="d-flex align-items-center">
                        {token ? (
                            <div className="d-flex gap-2 align-items-center">
                                {userRole === 'Admin' && (
                                    <span className="badge bg-danger text-white me-2">ADMIN</span>
                                )}
                                <button onClick={handleLogout} className="btn btn-outline-danger btn-sm">
                                    Wyloguj
                                </button>
                            </div>
                        ) : (
                            <Link to="/login" className="btn btn-primary btn-sm">
                                Zaloguj
                            </Link>
                        )}
                    </div>
                </div>
            </div>
        </nav>
    );
};

export default Navbar;