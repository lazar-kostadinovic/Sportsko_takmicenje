function getParametarIzUrla(parametar) {
    const urlSearchParams = new URLSearchParams(window.location.search);
    return urlSearchParams.get(parametar);
}

const takmicenjeId = getParametarIzUrla("takmicenjeId");

async function prikaziLeaderboardTakmicenja() {
    try {
        const response = await fetch(`http://localhost:5064/api/LeaderboardTakmicenje/${takmicenjeId}`);

        if (!response.ok) {
            throw new Error(`Greška prilikom preuzimanja leaderboard-a za takmičenje ${takmicenjeId}`);
        }

        const data = await response.json();

        const leaderboardTable = document.getElementById("leaderboardTable");
        const tbody = leaderboardTable.querySelector("tbody");
        tbody.innerHTML = ""; 

        for (const entry of data) {
            const nazivTima = await getTimById(entry.timId);

            const newRow = document.createElement("tr");

            const nazivTimaCell = document.createElement("td");
            nazivTimaCell.textContent = nazivTima;
            newRow.appendChild(nazivTimaCell);
        
            const pobedeCell = document.createElement("td");
            pobedeCell.textContent = entry.pobede;
            newRow.appendChild(pobedeCell);

            tbody.appendChild(newRow);
        }
    } catch (error) {
        console.error(error);
    }
}

async function azurirajLeaderboard() {
    try {
        const utakmicaIdsResponse = await fetch(`http://localhost:5064/api/Takmicenje/get-utakmiceID/${takmicenjeId}`);
        if (!utakmicaIdsResponse.ok) throw new Error("Neuspešno preuzimanje utakmica ID-ova.");

        const utakmicaIds = await utakmicaIdsResponse.json();

        for (const utakmicaId of utakmicaIds) {
            const timIdsResponse = await fetch(`http://localhost:5064/api/Utakmica/teamIds/${utakmicaId}`);
            if (!timIdsResponse.ok) throw new Error(`Neuspešno preuzimanje tim ID-ova za utakmicu ${utakmicaId}.`);

            const timIds = await timIdsResponse.json();

            for (const timId of timIds) {
                const leaderboardResponse = await fetch(
                    `http://localhost:5064/api/LeaderboardUtakmica/${utakmicaId}/${timId}`,
                    { method: "POST" }
                );

                if (!leaderboardResponse.ok) {
                    console.error(`Neuspešno ažuriranje leaderboard-a za tim ${timId} i utakmicu ${utakmicaId}`);
                }
            }

            const increaseWinsResponse = await fetch(
                `http://localhost:5064/api/LeaderboardTakmicenje/${utakmicaId}`,
                { method: "POST" }
            );
            
            const responseText = await increaseWinsResponse.text();
            console.log(`Odgovor API-ja za utakmicu ${utakmicaId}:`, responseText);
            

            if (!increaseWinsResponse.ok) {
                throw new Error(`Neuspešno povećavanje pobeda za utakmicu ${utakmicaId}.`);
            }

            console.log(`Uspešno ažuriran leaderboard za utakmicu ${utakmicaId}`);
        }
    } catch (error) {
        console.error("Greška prilikom ažuriranja leaderboard-a:", error);
    }
}

async function getTimById(id) {
    try {
        const response = await fetch(`http://localhost:5064/api/Tim/get-tim/${id}`);
        if (!response.ok) throw new Error(`Neuspešno preuzimanje tima sa ID ${id}.`);

        const timInfo = await response.json();
        return timInfo.naziv || "Nepoznat tim";
    } catch (error) {
        console.error(error);
        return "Nepoznat tim";
    }
}

async function inicijalizujLeaderboard() {
    if (!takmicenjeId) {
        console.error("takmicenjeId nije pronađen u URL-u.");
        return;
    }

    await azurirajLeaderboard();
    await prikaziLeaderboardTakmicenja();
}

inicijalizujLeaderboard();
