// DirksClientLibraryExample.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "DirkaLurkLibraryClient.h"

#include <string>
#include <cstdlib>
#include <sstream>
#include <chrono>
#include <iostream>

namespace DirkaLurk { namespace Library {
    class Client::Impl final
    {
    public:
        explicit Impl(std::string id) 
            : id(std::move(id))
        {
        }
        ~Impl() = default;

        void DoTheThing(void (__stdcall* function)(const char*), const char* message)
        {
            (*function)(message);
        }

        std::uint8_t  GenerateRandom8Bit()
        {
            return rand() % 256;
        }

        std::uint16_t GenerateRandom16Bit()
        {
            return rand() % 65536;
        }

        std::uint32_t GenerateRandom32Bit()
        {
            return rand() % 4294967296;
        }

        std::uint64_t GenerateRandom64Bit()
        {
            return rand(); // RAND_MAX == 0x7fff
        }

        std::string GenerateRandomUUID()
        {
            std::stringstream ss;

            for (auto i = 0; i < 8; i++) { ss << RandomHexi(); }
            ss << "-";
            for (auto i = 0; i < 4; i++) { ss << RandomHexi(); }
            ss << "-";
            for (auto i = 0; i < 4; i++) { ss << RandomHexi(); }
            ss << "-";
            for (auto i = 0; i < 4; i++) { ss << RandomHexi(); }
            ss << "-";
            for (auto i = 0; i < 12; i++) { ss << RandomHexi(); }

            return ss.str();
        }

        char GenerateRandomChar()
        {
            return RandomChar();
        }

        std::string GetId()
        {
            return id;
        }

    private:
        const std::string id;

        static char RandomChar()
        {
            return 'a' + (rand() % 26);
        };

        static char RandomHexi()
        {
            int r = rand() % 16;

            return (r < 10)
                ? '0' + r
                : 'a' + (r % 10);
        };
    };

    Client::Client(std::string id)
        : impl(new Impl(std::move(id)))
    {
    }


    void Client::DoTheThing(void (__stdcall* function)(const char*), const char* message) const
    {
        GetImpl()->DoTheThing(function, message);
    }

    std::uint8_t Client::GenerateRandom8Bit() const
    {
        return GetImpl()->GenerateRandom8Bit();
    }

    std::uint16_t Client::GenerateRandom16Bit() const
    {
        return GetImpl()->GenerateRandom16Bit();
    }

    std::uint32_t Client::GenerateRandom32Bit() const
    {
        return GetImpl()->GenerateRandom32Bit();
    }

    std::uint64_t Client::GenerateRandom64Bit() const
    {
        return GetImpl()->GenerateRandom64Bit();
    }

    char Client::GenerateRandomChar() const
    {
        return GetImpl()->GenerateRandomChar();
    }

    std::string Client::GenerateRandomUUID() const
    {
        return GetImpl()->GenerateRandomUUID();
    }

    std::string Client::GetId() const
    {
        return GetImpl()->GetId();
    }
}
}


//~ ============================================================== ~//
//  EXPOSE TO DLL
//~ ============================================================== ~//

static const auto client = std::make_unique<DirkaLurk::Library::Client>("steve");

std::uint8_t GenerateRandom8Bit()
{
    return client->GenerateRandom8Bit();
}

std::uint16_t GenerateRandom16Bit()
{
    return client->GenerateRandom16Bit();
}

std::uint32_t GenerateRandom32Bit()
{
    return client->GenerateRandom32Bit();
}

std::uint64_t GenerateRandom64Bit()
{
    return client->GenerateRandom64Bit();
}

char GenerateRandomChar()
{
    return client->GenerateRandomChar();
}

void GenerateRandomString(OUT char* str, const int length)
{
    for (auto i = 0; i < length; i++)
    {
        str[i] = client->GenerateRandomChar();
    }
}

void GenerateRandomUUID(OUT char* uuid)
{
    std::string uuidStr = client->GenerateRandomUUID();
    for (auto i = 0; i < uuidStr.length(); i++)
    {
        uuid[i] = uuidStr[i];
    }
}

char ToUpperChar(const char c)
{
    return c - 32;
}

void ToUpperString(OUT char* lower, const char* input, const int length)
{
    // also, an on-stack copy
    for (auto i = 0; i < length; i++)
    {
        lower[i] = input[i] - 32;
    }
}

void InvokeMe(void (__stdcall* function)(const char*), const char* message, int length)
{
    std::string msg(message, length);
    auto upperMessage = msg.append(client->GenerateRandomUUID());

    client->DoTheThing(function, upperMessage.c_str());
}

void(__stdcall* delayFunction)(void);
std::chrono::milliseconds functionDelayMillis;
std::chrono::steady_clock::time_point delayStartTime;

bool delaySet = false;

void DelayInvoke(void(__stdcall* function)(), int delayMillis)
{
    delayFunction = function;
    functionDelayMillis = std::chrono::milliseconds(delayMillis);

    delayStartTime = std::chrono::steady_clock::now();

    delaySet = true;
}

void PollInvoke()
{
    if (!delaySet) { return; }

    //std::cout << "Polling..." << std::endl;

    if (functionDelayMillis < (std::chrono::steady_clock::now() - delayStartTime))
    {
        (*delayFunction)();
        delaySet = false;
    }
}