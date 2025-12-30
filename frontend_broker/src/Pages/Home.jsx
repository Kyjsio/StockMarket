import React from 'react';
import { useNavigate } from 'react-router-dom';

const Home = () => {
    const navigate = useNavigate();

    const handleNavigation = (targetPath) => {
        const token = localStorage.getItem('token'); 

        if (token) {
            navigate(targetPath);
        } else {
            navigate('/login');
        }
    };

    return (
        <div className="d-flex flex-column justify-content-center align-items-center h-100 text-center">

            <div className="card shadow-lg border-0 p-5 rounded-3" style={{ maxWidth: '700px', width: '100%' }}>

                <h1 className="display-4 fw-bold text-primary mb-4">Broker App</h1>

                <p className="lead text-secondary mb-5">
                    Profesjonalne narzędzie do zarządzania portfelem inwestycyjnym.
                    Analizuj dane rynkowe z AlphaVantage i inwestuj wirtualne środki.
                </p>

                <div className="d-flex flex-column flex-sm-row gap-3 justify-content-center">

                    <button
                        onClick={() => handleNavigation('/market')}
                        className="btn btn-primary btn-lg px-4 fw-bold shadow-sm"
                    >
                        Przejdź do Rynku
                    </button>

                    <button
                        onClick={() => handleNavigation('/portfolio')}
                        className="btn btn-outline-primary btn-lg px-4 fw-bold shadow-sm"
                    >
                        Twój Portfel
                    </button>
                </div>
            </div>
        </div>
    );
};

export default Home;